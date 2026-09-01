using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CompanyController : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 12f;
    public float rotationSpeed = 12f;
    public float stoppingDistance = 0.2f;

    [Header("Animación")]
    public Animator characterAnimator;
    public string movingParamName = "isMoving";
    public string speedParamName = "Speed";

    [Header("Capa del Suelo (Para Clic)")]
    public LayerMask groundLayerMask = ~0;

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool isMovingToTarget = false;
    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
    }

    private void Start()
    {
        mainCamera = Camera.main;

        if (characterAnimator == null)
        {
            characterAnimator = GetComponentInChildren<Animator>();
        }

        RestoreWorldPosition();
    }

    private void Update()
    {
        HandleMouseInput();
        ExecuteMovement();
        UpdateAnimation();
    }

    private void HandleMouseInput()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        // Detectar si el ratón está sobre la UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        bool mousePressed = false;
        Vector2 mouseScreenPos = Vector2.zero;

        if (Mouse.current != null)
        {
            // Clic izquierdo o derecho para moverse, o mantener presionado para guiar
            mousePressed = Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed;
            mouseScreenPos = Mouse.current.position.ReadValue();
        }
#if !ENABLE_INPUT_SYSTEM
        else
        {
            mousePressed = Input.GetMouseButton(0) || Input.GetMouseButton(1);
            mouseScreenPos = Input.mousePosition;
        }
#endif

        if (mousePressed)
        {
            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);

            // Raycast contra el suelo o un plano horizontal a la altura actual
            if (Physics.Raycast(ray, out RaycastHit hit, 500f, groundLayerMask))
            {
                SetDestination(hit.point);
            }
            else
            {
                // Fallback: intersección con plano Y = 0
                Plane groundPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
                if (groundPlane.Raycast(ray, out float enter))
                {
                    SetDestination(ray.GetPoint(enter));
                }
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        destination.y = transform.position.y;
        targetPosition = destination;
        isMovingToTarget = true;
    }

    private void ExecuteMovement()
    {
        if (!isMovingToTarget)
            return;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetPosition;
        targetPos.y = currentPos.y;

        Vector3 direction = targetPos - currentPos;
        float distance = direction.magnitude;

        if (distance <= stoppingDistance)
        {
            isMovingToTarget = false;
            return;
        }

        Vector3 moveDir = direction.normalized;

        // Rotación suave hacia la dirección de movimiento
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Movimiento
        if (rb != null && rb.isKinematic)
        {
            rb.MovePosition(rb.position + moveDir * speed * Time.deltaTime);
        }
        else
        {
            transform.position += moveDir * speed * Time.deltaTime;
        }
    }

    private void UpdateAnimation()
    {
        if (characterAnimator == null)
            return;

        characterAnimator.SetBool(movingParamName, isMovingToTarget);
        characterAnimator.SetFloat(speedParamName, isMovingToTarget ? speed : 0f);
    }

    private void RestoreWorldPosition()
    {
        if (GameManager.Instance == null)
            return;

        if (!GameManager.Instance.HasCompanyWorldPosition)
            return;

        transform.position = GameManager.Instance.CompanyWorldPosition;
        targetPosition = transform.position;
        isMovingToTarget = false;

        if (rb != null)
        {
            rb.position = GameManager.Instance.CompanyWorldPosition;
        }

        Debug.Log("Posición de compañía restaurada: " + transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        if (isMovingToTarget)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
        }
    }
}
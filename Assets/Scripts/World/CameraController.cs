using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;
    [SerializeField] private bool followTarget = true;

    [Header("Movimiento WASD")]
    public float moveSpeed = 20f;
    public float followSpeed = 8f;

    [Header("Zoom con Rueda del Ratón")]
    [Tooltip("Cantidad de zoom por cada 'click' de la rueda del ratón")]
    public float zoomStep = 3.5f;
    [Tooltip("Suavizado de la transición del zoom")]
    public float zoomSmoothing = 15f;

    public float minHeight = 6f;
    public float maxHeight = 35f;

    public float minDistance = 4f;
    public float maxDistance = 25f;

    private float currentHeight = 15f;
    private float currentDistance = 10f;
    private float targetHeight = 15f;
    private float targetDistance = 10f;

    private void Start()
    {
        currentHeight = transform.position.y;
        currentDistance = target != null ? (target.position.z - transform.position.z) : 10f;

        currentHeight = Mathf.Clamp(currentHeight, minHeight, maxHeight);
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);

        targetHeight = currentHeight;
        targetDistance = currentDistance;
    }

    private void Update()
    {
        HandleZoom();
        HandleTargetToggle();
        HandlePanAndFollow();
    }

    private void HandleZoom()
    {
        float scroll = 0f;

        if (Mouse.current != null)
        {
            scroll = Mouse.current.scroll.ReadValue().y;
        }
#if !ENABLE_INPUT_SYSTEM
        if (Mathf.Abs(scroll) < 0.01f)
        {
            scroll = Input.GetAxis("Mouse ScrollWheel") * 100f;
        }
#endif

        if (Mathf.Abs(scroll) > 0.01f)
        {
            // scroll > 0 = Acercar (Zoom In), scroll < 0 = Alejar (Zoom Out)
            float scrollDir = Mathf.Sign(scroll);

            targetHeight -= scrollDir * zoomStep;
            targetDistance -= scrollDir * zoomStep * 0.7f;

            targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
            targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
        }

        // Interpolar suavemente hacia la altura y distancia objetivo
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * zoomSmoothing);
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * zoomSmoothing);
    }

    private void HandleTargetToggle()
    {
        if (Keyboard.current != null)
        {
            // Espacio o F para volver a enfocar la cámara en la compañía
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.fKey.wasPressedThisFrame)
            {
                followTarget = true;
            }
        }
    }

    private void HandlePanAndFollow()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
        }

        // Si el jugador presiona WASD, desactivar el seguimiento automático y mover la cámara libremente
        if (input.sqrMagnitude > 0.001f)
        {
            followTarget = false;

            Vector3 panMovement = new Vector3(input.x, 0f, input.y).normalized * moveSpeed * Time.deltaTime;
            Vector3 newPos = transform.position + panMovement;
            newPos.y = currentHeight;
            transform.position = newPos;
            return;
        }

        // Si el seguimiento está activado y hay objetivo
        if (followTarget && target != null)
        {
            Vector3 desiredPosition = new Vector3(
                target.position.x,
                currentHeight,
                target.position.z - currentDistance
            );

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                followSpeed * Time.deltaTime
            );
        }
        else
        {
            // Mantener la altura deseada durante la navegación libre
            Vector3 pos = transform.position;
            pos.y = currentHeight;
            transform.position = pos;
        }
    }
}
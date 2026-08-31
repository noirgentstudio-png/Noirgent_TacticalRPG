using UnityEngine;
using UnityEngine.InputSystem;

public class CompanyController : MonoBehaviour
{
    public float speed = 12f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        RestoreWorldPosition();
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        Vector2 movement = Vector2.zero;

        if (Keyboard.current.upArrowKey.isPressed)
            movement.y += 1;

        if (Keyboard.current.downArrowKey.isPressed)
            movement.y -= 1;

        if (Keyboard.current.leftArrowKey.isPressed)
            movement.x -= 1;

        if (Keyboard.current.rightArrowKey.isPressed)
            movement.x += 1;

        if (movement.sqrMagnitude > 0.001f)
        {
            Vector3 direction = new Vector3(movement.x, 0, movement.y).normalized;

            if (rb != null && rb.isKinematic)
            {
                rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
            }
            else
            {
                transform.position += direction * speed * Time.deltaTime;
            }
        }
    }

    private void RestoreWorldPosition()
    {
        if (GameManager.Instance == null)
            return;

        if (!GameManager.Instance.HasCompanyWorldPosition)
            return;

        transform.position = GameManager.Instance.CompanyWorldPosition;
        if (rb != null)
        {
            rb.position = GameManager.Instance.CompanyWorldPosition;
        }

        Debug.Log("Posición de compañía restaurada: " + transform.position);
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class CompanyController : MonoBehaviour
{
    public float speed = 5f;

    private void Start()
    {
        RestoreWorldPosition();
    }

    private void Update()
    {
        Vector2 movement = Vector2.zero;

        if (Keyboard.current.upArrowKey.isPressed)
            movement.y += 1;

        if (Keyboard.current.downArrowKey.isPressed)
            movement.y -= 1;

        if (Keyboard.current.leftArrowKey.isPressed)
            movement.x -= 1;

        if (Keyboard.current.rightArrowKey.isPressed)
            movement.x += 1;

        Vector3 direction = new Vector3(movement.x, 0, movement.y);

        transform.position += direction * speed * Time.deltaTime;
    }

    private void RestoreWorldPosition()
    {
        if (GameManager.Instance == null)
            return;

        if (!GameManager.Instance.HasCompanyWorldPosition)
            return;

        transform.position = GameManager.Instance.CompanyWorldPosition;

        Debug.Log("Posición de compañía restaurada: " + transform.position);
    }
}
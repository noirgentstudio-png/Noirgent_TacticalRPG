using UnityEngine;
using UnityEngine.InputSystem;

public class TownCameraController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 12f;

    [Header("Zoom")]
    public float zoomSpeed = 10f;
    public float minHeight = 8f;
    public float maxHeight = 25f;

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Eliminamos la componente vertical para movernos
        // únicamente sobre el plano de la ciudad.
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 movement = right * input.x + forward * input.y;

        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll == 0)
            return;

        Vector3 position = transform.position;

        position.y -= scroll * zoomSpeed * Time.deltaTime;

        position.y = Mathf.Clamp(position.y, minHeight, maxHeight);

        transform.position = position;
    }
}
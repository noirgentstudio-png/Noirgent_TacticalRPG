using UnityEngine;
using UnityEngine.InputSystem;

public class TownCameraController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 15f;

    [Header("Zoom")]
    public float zoomStep = 2.5f;
    public float zoomSmoothing = 15f;
    public float minHeight = 6f;
    public float maxHeight = 28f;

    private float targetHeight = 15f;
    private float currentHeight = 15f;

    private void Start()
    {
        currentHeight = transform.position.y;
        targetHeight = currentHeight;
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 movement = (right * input.x + forward * input.y).normalized;

        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scroll = 0f;

        if (Mouse.current != null)
        {
            scroll = Mouse.current.scroll.ReadValue().y;
        }

        if (Mathf.Abs(scroll) > 0.01f)
        {
            float scrollDir = Mathf.Sign(scroll);
            targetHeight -= scrollDir * zoomStep;
            targetHeight = Mathf.Clamp(targetHeight, minHeight, maxHeight);
        }

        currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * zoomSmoothing);

        Vector3 position = transform.position;
        position.y = currentHeight;
        transform.position = position;
    }
}
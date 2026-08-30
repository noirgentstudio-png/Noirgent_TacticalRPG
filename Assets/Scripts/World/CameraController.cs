using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float moveSpeed = 12f;
    public float followSpeed = 8f;

    public float cameraHeight = 15f;
    public float cameraDistance = 10f;
    public float zoomSpeed = 20f;
    public float minHeight = 8f;
    public float maxHeight = 30f;

    public float minDistance = 5f;
    public float maxDistance = 22f;

    private bool followTarget = true;

    void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            cameraHeight -= scroll * zoomSpeed * Time.deltaTime;
            cameraDistance -= scroll * zoomSpeed * 0.7f * Time.deltaTime;

            cameraHeight = Mathf.Clamp(cameraHeight, minHeight, maxHeight);
            cameraDistance = Mathf.Clamp(cameraDistance, minDistance, maxDistance);
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            followTarget = !followTarget;
        }

        if (followTarget && target != null)
        {
            Vector3 desiredPosition = new Vector3(
                target.position.x,
                cameraHeight,
                target.position.z - cameraDistance);

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                followSpeed * Time.deltaTime);

            return;
        }

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            input.y += 1;

        if (Keyboard.current.sKey.isPressed)
            input.y -= 1;

        if (Keyboard.current.dKey.isPressed)
            input.x += 1;

        if (Keyboard.current.aKey.isPressed)
            input.x -= 1;

        Vector3 movement = new Vector3(input.x, 0f, input.y);

        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}
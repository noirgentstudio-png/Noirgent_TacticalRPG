using UnityEngine;
using UnityEngine.InputSystem;

public class CombatCameraController : MonoBehaviour
{
    [Header("Velocidad de movimiento")]
    public float panSpeed = 15f;
    public float zoomStep = 2.5f;
    public float zoomSmoothing = 15f;

    [Header("Límites de Zoom")]
    public float minZoom = 5f;
    public float maxZoom = 25f;

    [Header("Límites de desplazamiento")]
    public Vector2 panLimitX = new Vector2(-20f, 20f);
    public Vector2 panLimitZ = new Vector2(-20f, 20f);

    private float targetZoom = 12f;
    private float currentZoom = 12f;

    private void Awake()
    {
        currentZoom = transform.position.y;
        targetZoom = currentZoom;
    }

    private void Update()
    {
        HandlePan();
        HandleZoom();
    }

    private void HandlePan()
    {
        Vector3 move = Vector3.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) move.z += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) move.z -= 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move.x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move.x += 1f;
        }

        if (move.sqrMagnitude > 0.001f)
        {
            Vector3 targetPos = transform.position + move.normalized * panSpeed * Time.deltaTime;
            targetPos.x = Mathf.Clamp(targetPos.x, panLimitX.x, panLimitX.y);
            targetPos.z = Mathf.Clamp(targetPos.z, panLimitZ.x, panLimitZ.y);
            targetPos.y = currentZoom;

            transform.position = targetPos;
        }
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
            targetZoom -= scrollDir * zoomStep;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomSmoothing);

        Vector3 pos = transform.position;
        pos.y = currentZoom;
        transform.position = pos;
    }
}

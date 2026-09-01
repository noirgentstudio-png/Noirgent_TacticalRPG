using UnityEngine;
using UnityEngine.InputSystem;

public class CombatCameraController : MonoBehaviour
{
    [Header("Velocidad de movimiento")]
    public float panSpeed = 12f;
    public float zoomSpeed = 5f;

    [Header("Límites de Zoom")]
    public float minZoom = 6f;
    public float maxZoom = 20f;

    [Header("Límites de desplazamiento")]
    public Vector2 panLimitX = new Vector2(-15f, 15f);
    public Vector2 panLimitZ = new Vector2(-15f, 15f);

    private Camera cam;
    private float currentZoom = 12f;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        currentZoom = transform.position.y;
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
            targetPos.y = transform.position.y;

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
            currentZoom -= (scroll > 0 ? 1f : -1f) * zoomSpeed * Time.deltaTime * 5f;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            Vector3 pos = transform.position;
            pos.y = Mathf.Lerp(pos.y, currentZoom, Time.deltaTime * 10f);
            transform.position = pos;
        }
    }
}


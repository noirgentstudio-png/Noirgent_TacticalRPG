using UnityEngine;
using UnityEngine.InputSystem;

public class MarketCameraController : MonoBehaviour
{
    [Header("Sensibilidad")]
    [SerializeField] private float sensitivity = 0.15f;

    [Header("Límites horizontales")]
    [SerializeField] private float horizontalLimit = 85f;

    [Header("Límites verticales")]
    [SerializeField] private float lookUpLimit = 25f;
    [SerializeField] private float lookDownLimit = 15f;

    private float currentYaw = 0f;
    private float currentPitch = 0f;

    private void Start()
    {
        currentYaw = transform.localEulerAngles.y;
        currentPitch = transform.localEulerAngles.x;

        if (currentYaw > 180f)
            currentYaw -= 360f;

        if (currentPitch > 180f)
            currentPitch -= 360f;
    }

    private void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        currentYaw += mouseDelta.x * sensitivity;
        currentPitch -= mouseDelta.y * sensitivity;

        currentYaw = Mathf.Clamp(
            currentYaw,
            -horizontalLimit,
            horizontalLimit
        );

        currentPitch = Mathf.Clamp(
            currentPitch,
            -lookUpLimit,
            lookDownLimit
        );

        transform.localRotation = Quaternion.Euler(
            currentPitch,
            currentYaw,
            0f
        );
    }
}
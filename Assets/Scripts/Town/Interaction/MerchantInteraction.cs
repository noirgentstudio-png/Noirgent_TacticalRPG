using UnityEngine;
using UnityEngine.InputSystem;

public class MerchantInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject marketCanvas;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        HandleMouseClick();
        HandleCanvasClose();
    }

    private void HandleMouseClick()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (playerCamera == null)
            {
                playerCamera = Camera.main;
                if (playerCamera == null) return;
            }

            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("COMERCIANTE SELECCIONADO: Abriendo panel de mercado");

                    if (marketCanvas != null)
                    {
                        marketCanvas.SetActive(true);
                    }
                }
            }
        }
    }

    private void HandleCanvasClose()
    {
        if (marketCanvas == null || !marketCanvas.activeSelf)
            return;

        // Si la interfaz del mercado está abierta y el jugador presiona ESC o clic derecho, cerrar el panel de comercio
        bool closeKey = false;
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            closeKey = true;
        }
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            closeKey = true;
        }

        if (closeKey)
        {
            Debug.Log("Cerrando panel de comercio.");
            marketCanvas.SetActive(false);
        }
    }

    public void CloseMarket()
    {
        if (marketCanvas != null)
        {
            marketCanvas.SetActive(false);
        }
    }
}
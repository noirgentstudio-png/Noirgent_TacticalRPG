using UnityEngine;
using UnityEngine.InputSystem;

public class MerchantInteraction : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject marketCanvas;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("COMERCIANTE SELECCIONADO");

                    marketCanvas.SetActive(true);
                }
            }
        }
    }
}
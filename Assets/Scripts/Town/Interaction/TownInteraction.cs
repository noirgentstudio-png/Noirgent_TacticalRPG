using UnityEngine;
using UnityEngine.InputSystem;

public class TownInteraction : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private TownTooltip tooltip;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            InteractionInfo info = hit.collider.GetComponent<InteractionInfo>();

            if (info != null)
            {
                tooltip.Show(
                    info.displayName,
                    info.description,
                    mousePosition
                );

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Debug.Log("CLIC DETECTADO SOBRE: " + hit.collider.gameObject.name);

                    ITownInteractable interactable =
                        hit.collider.GetComponent<ITownInteractable>();

                    if (interactable != null)
                    {
                        Debug.Log("INTERACTABLE ENCONTRADO");
                        interactable.Interact();
                    }
                    else
                    {
                        Debug.Log("EL OBJETO NO IMPLEMENTA ITownInteractable");
                    }
                }

                return;
            }
        }

        tooltip.Hide();
    }
}
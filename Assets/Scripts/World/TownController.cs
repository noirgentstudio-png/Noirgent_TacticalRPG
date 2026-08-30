using UnityEngine;
using UnityEngine.InputSystem;

public class TownController : MonoBehaviour
{
    public string townName = "Town_Test";

    private bool playerInsideTown = false;
    private bool enteringTown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Company"))
        {
            playerInsideTown = true;
            enteringTown = false;

            UIManager.Instance.ShowTownPanel(townName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Company"))
        {
            playerInsideTown = false;
            enteringTown = false;

            UIManager.Instance.HideTownPanel();

            GameManager.Instance.ExitTown();
        }
    }

    private void Update()
    {
        if (!playerInsideTown)
            return;

        if (enteringTown)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            enteringTown = true;

            GameManager.Instance.EnterTown(townName);
        }
    }
}
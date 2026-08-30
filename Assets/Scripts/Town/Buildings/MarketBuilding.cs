using UnityEngine;

public class MarketBuilding : MonoBehaviour, ITownInteractable
{
    [Header("Configuración del edificio")]
    [SerializeField] private string sceneName = "MarketScene";

    public void Interact()
    {
        Debug.Log("INTERACTUANDO CON EL MERCADO");

        if (TownBuildingSystem.Instance == null)
        {
            Debug.LogError("No existe un TownBuildingSystem en la escena.");
            return;
        }

        TownBuildingSystem.Instance.EnterBuilding(sceneName);
    }
}
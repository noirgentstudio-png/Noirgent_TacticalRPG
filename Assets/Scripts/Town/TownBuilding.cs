using UnityEngine;

public abstract class TownBuilding : MonoBehaviour, ITownInteractable
{
    [Header("Identidad")]
    [SerializeField] private string buildingID = "building";
    [SerializeField] private string displayName = "Edificio";
    [SerializeField] private string description = "";

    public string BuildingID => buildingID;
    public string DisplayName => displayName;
    public string Description => description;

    public abstract void Interact();
}
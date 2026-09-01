using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [Header("Referencias")]
    public HexGrid hexGrid;
    public TextMeshProUGUI combatInfoText;
    public TextMeshProUGUI tileInfoText;

    [Header("Materiales de Unidades")]
    public Material playerUnitMaterial;
    public Material enemyUnitMaterial;

    private List<GameObject> spawnedUnits = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (hexGrid == null)
        {
            hexGrid = FindFirstObjectByType<HexGrid>();
        }

        if (hexGrid != null)
        {
            hexGrid.OnTileHovered += HandleTileHovered;
            hexGrid.OnTileClicked += HandleTileClicked;
        }

        if (combatInfoText != null)
        {
            combatInfoText.text = "Fase de Despliegue: Selecciona una casilla para inspeccionar.";
        }

        StartCoroutine(SpawnInitialUnitsAfterGrid());
    }

    private IEnumerator SpawnInitialUnitsAfterGrid()
    {
        yield return null; // Esperar 1 frame para que la cuadrícula se genere

        SpawnUnit(0, 3, true, "Guerrero Compañía");
        SpawnUnit(0, 4, true, "Ballestero Compañía");
        SpawnUnit(1, 3, true, "Escudero Compañía");

        int enemyCol = hexGrid != null ? hexGrid.width - 1 : 11;
        SpawnUnit(enemyCol, 3, false, "Líder Bandido");
        SpawnUnit(enemyCol, 4, false, "Arquero Forajido");
        SpawnUnit(enemyCol - 1, 4, false, "Rufián");
    }

    private void SpawnUnit(int col, int row, bool isPlayer, string unitName)
    {
        if (hexGrid == null) return;

        HexTile tile = hexGrid.GetTile(col, row);
        if (tile == null) return;

        GameObject unit = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        unit.name = unitName;
        unit.transform.position = tile.transform.position + Vector3.up * 0.5f;
        unit.transform.localScale = new Vector3(0.6f, 0.4f, 0.6f);

        // Remover o ajustar collider para que no bloquee raycast del tile
        Collider colComp = unit.GetComponent<Collider>();
        if (colComp != null) Destroy(colComp);

        MeshRenderer mr = unit.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            Material mat = isPlayer ? playerUnitMaterial : enemyUnitMaterial;
            if (mat != null)
            {
                mr.sharedMaterial = mat;
            }
            else
            {
                Material newMat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                newMat.color = isPlayer ? new Color(0.2f, 0.5f, 0.9f) : new Color(0.9f, 0.2f, 0.2f);
                mr.material = newMat;
            }
        }

        spawnedUnits.Add(unit);
    }

    private void HandleTileHovered(HexTile tile)
    {
        if (tileInfoText != null && tile != null)
        {
            tileInfoText.text = $"Casilla: ({tile.Coordinates.x}, {tile.Coordinates.y}) | Estado: {tile.CurrentState}";
        }
    }

    private void HandleTileClicked(HexTile tile)
    {
        if (combatInfoText != null && tile != null)
        {
            combatInfoText.text = $"Casilla Seleccionada: [{tile.Coordinates.x}, {tile.Coordinates.y}]";
        }
    }

    private void Update()
    {
        // Tecla ESC para retirarse / regresar al mapa
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            RetreatCombat();
        }
    }

    public void RetreatCombat()
    {
        Debug.Log("Saliendo del combate y regresando al mapa del mundo...");

        string returnScene = "WorldPrototype";
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.PreviousScene))
        {
            returnScene = GameManager.Instance.PreviousScene;
        }

        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(returnScene);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(returnScene);
        }
    }
}


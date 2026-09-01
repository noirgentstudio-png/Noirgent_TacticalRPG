using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HexGrid : MonoBehaviour
{
    [Header("Dimensiones de la Cuadrícula")]
    public int width = 12;
    public int height = 8;

    [Header("Material Base")]
    public Material tileMaterial;

    [Header("Capas para Raycast")]
    public LayerMask tileLayerMask = ~0;

    private Dictionary<Vector2Int, HexTile> tiles = new Dictionary<Vector2Int, HexTile>();
    private HexTile currentHoveredTile;
    private HexTile selectedTile;

    public event System.Action<HexTile> OnTileHovered;
    public event System.Action<HexTile> OnTileClicked;

    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        HandleMouseInteraction();
    }

    public void GenerateGrid()
    {
        ClearGrid();

        if (tileMaterial == null)
        {
            tileMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
        }

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateTile(x, z);
            }
        }

        CenterGrid();
    }

    private void CreateTile(int x, int z)
    {
        GameObject tileObj = new GameObject($"Hex_{x}_{z}");
        tileObj.transform.SetParent(transform);

        MeshRenderer mr = tileObj.AddComponent<MeshRenderer>();
        mr.sharedMaterial = tileMaterial;

        HexTile tile = tileObj.AddComponent<HexTile>();
        tile.Initialize(x, z);

        // Zonas de despliegue iniciales
        if (x <= 1)
        {
            tile.SetState(HexTileState.DeploymentPlayer);
        }
        else if (x >= width - 2)
        {
            tile.SetState(HexTileState.DeploymentEnemy);
        }

        tiles[new Vector2Int(x, z)] = tile;
    }

    private void CenterGrid()
    {
        // Calcular el centro geométrico para centrar la cuadrícula en (0, 0, 0)
        Vector3 minPos = HexMetrics.GetWorldPosition(0, 0);
        Vector3 maxPos = HexMetrics.GetWorldPosition(width - 1, height - 1);
        Vector3 centerOffset = (minPos + maxPos) * 0.5f;

        transform.position = -centerOffset;
    }

    private void HandleMouseInteraction()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null)
            return;

        Vector2 mousePos = Vector2.zero;
        if (Mouse.current != null)
        {
            mousePos = Mouse.current.position.ReadValue();
        }
#if !ENABLE_INPUT_SYSTEM
        else
        {
            mousePos = Input.mousePosition;
        }
#endif

        Ray ray = mainCam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileLayerMask))
        {
            HexTile tile = hit.collider.GetComponent<HexTile>();
            if (tile != null)
            {
                if (currentHoveredTile != tile)
                {
                    // Desmarcar anterior
                    if (currentHoveredTile != null && currentHoveredTile != selectedTile)
                    {
                        RestoreTileState(currentHoveredTile);
                    }

                    currentHoveredTile = tile;

                    // Marcar nuevo hover
                    if (currentHoveredTile != selectedTile)
                    {
                        currentHoveredTile.SetState(HexTileState.Hovered);
                    }

                    OnTileHovered?.Invoke(currentHoveredTile);
                }

                // Clic izquierdo para seleccionar
                bool isClick = (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);
                if (isClick)
                {
                    SelectTile(tile);
                }

                return;
            }
        }

        // Si el ratón no apunta a ninguna casilla
        if (currentHoveredTile != null)
        {
            if (currentHoveredTile != selectedTile)
            {
                RestoreTileState(currentHoveredTile);
            }
            currentHoveredTile = null;
        }
    }

    public void SelectTile(HexTile tile)
    {
        if (selectedTile != null)
        {
            RestoreTileState(selectedTile);
        }

        selectedTile = tile;

        if (selectedTile != null)
        {
            selectedTile.SetState(HexTileState.Selected);
            Debug.Log($"Hex seleccionado: {selectedTile.Coordinates.x}, {selectedTile.Coordinates.y}");
            OnTileClicked?.Invoke(selectedTile);
        }
    }

    private void RestoreTileState(HexTile tile)
    {
        if (tile == null) return;

        int x = tile.Coordinates.x;
        int z = tile.Coordinates.y;

        if (x <= 1)
        {
            tile.SetState(HexTileState.DeploymentPlayer);
        }
        else if (x >= width - 2)
        {
            tile.SetState(HexTileState.DeploymentEnemy);
        }
        else
        {
            tile.SetState(HexTileState.Default);
        }
    }

    public HexTile GetTile(int x, int z)
    {
        tiles.TryGetValue(new Vector2Int(x, z), out HexTile tile);
        return tile;
    }

    public List<HexTile> GetNeighbors(HexTile tile)
    {
        List<HexTile> neighborList = new List<HexTile>();
        if (tile == null) return neighborList;

        int parity = tile.Coordinates.y & 1;
        Vector2Int[] dirList = HexMetrics.neighbors[parity];

        foreach (Vector2Int dir in dirList)
        {
            Vector2Int neighborCoords = tile.Coordinates + dir;
            if (tiles.TryGetValue(neighborCoords, out HexTile nTile))
            {
                neighborList.Add(nTile);
            }
        }

        return neighborList;
    }

    private void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
        tiles.Clear();
        currentHoveredTile = null;
        selectedTile = null;
    }
}


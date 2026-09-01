using System.Collections.Generic;
using UnityEngine;

public enum HexTileState
{
    Default,
    Hovered,
    Selected,
    Reachable,
    Attackable,
    Obstacle,
    DeploymentPlayer,
    DeploymentEnemy
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class HexTile : MonoBehaviour
{
    public Vector2Int Coordinates { get; private set; }
    public HexTileState CurrentState { get; private set; } = HexTileState.Default;
    public bool IsWalkable { get; set; } = true;

    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propBlock;

    // Colores para estados visuales
    private static readonly Color ColorDefault = new Color(0.22f, 0.25f, 0.28f, 1f);
    private static readonly Color ColorHovered = new Color(0.45f, 0.75f, 0.95f, 1f);
    private static readonly Color ColorSelected = new Color(0.95f, 0.85f, 0.25f, 1f);
    private static readonly Color ColorReachable = new Color(0.25f, 0.60f, 0.90f, 1f);
    private static readonly Color ColorAttackable = new Color(0.85f, 0.25f, 0.25f, 1f);
    private static readonly Color ColorObstacle = new Color(0.12f, 0.12f, 0.14f, 1f);
    private static readonly Color ColorPlayerDeploy = new Color(0.18f, 0.40f, 0.65f, 1f);
    private static readonly Color ColorEnemyDeploy = new Color(0.65f, 0.22f, 0.22f, 1f);

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        propBlock = new MaterialPropertyBlock();
    }

    public void Initialize(int x, int z)
    {
        Coordinates = new Vector2Int(x, z);
        name = $"Hex_{x}_{z}";
        transform.localPosition = HexMetrics.GetWorldPosition(x, z);

        GenerateHexMesh();
        UpdateColor();
    }

    public void SetState(HexTileState newState)
    {
        CurrentState = newState;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        if (propBlock == null)
            propBlock = new MaterialPropertyBlock();

        Color targetColor = CurrentState switch
        {
            HexTileState.Hovered => ColorHovered,
            HexTileState.Selected => ColorSelected,
            HexTileState.Reachable => ColorReachable,
            HexTileState.Attackable => ColorAttackable,
            HexTileState.Obstacle => ColorObstacle,
            HexTileState.DeploymentPlayer => ColorPlayerDeploy,
            HexTileState.DeploymentEnemy => ColorEnemyDeploy,
            _ => ColorDefault
        };

        meshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor("_BaseColor", targetColor);
        propBlock.SetColor("_Color", targetColor);
        meshRenderer.SetPropertyBlock(propBlock);
    }

    /// <summary>
    /// Genera la malla 3D de un prisma hexagonal con un ligero bisel superior para destacar los bordes.
    /// </summary>
    private void GenerateHexMesh()
    {
        Mesh mesh = new Mesh { name = "HexTileMesh" };

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        float scale = 0.94f; // 94% del radio para dejar un canal visible entre casillas
        float height = 0.15f;

        // Vértice central superior
        Vector3 topCenter = new Vector3(0f, height, 0f);
        vertices.Add(topCenter);
        normals.Add(Vector3.up);

        // 6 vértices superiores
        for (int i = 0; i < 6; i++)
        {
            Vector3 vertex = HexMetrics.corners[i] * scale;
            vertex.y = height;
            vertices.Add(vertex);
            normals.Add(Vector3.up);
        }

        // Triángulos de la tapa superior
        for (int i = 1; i <= 6; i++)
        {
            int next = (i % 6) + 1;
            triangles.Add(0);
            triangles.Add(next);
            triangles.Add(i);
        }

        // Vértices y caras laterales
        for (int i = 0; i < 6; i++)
        {
            int nextIndex = (i + 1) % 6;
            Vector3 topA = HexMetrics.corners[i] * scale + Vector3.up * height;
            Vector3 topB = HexMetrics.corners[nextIndex] * scale + Vector3.up * height;
            Vector3 botA = HexMetrics.corners[i] * scale;
            Vector3 botB = HexMetrics.corners[nextIndex] * scale;

            Vector3 sideNormal = Vector3.Cross(topB - topA, botA - topA).normalized;

            int baseIdx = vertices.Count;
            vertices.Add(topA);
            vertices.Add(topB);
            vertices.Add(botB);
            vertices.Add(botA);

            for (int k = 0; k < 4; k++) normals.Add(sideNormal);

            triangles.Add(baseIdx);
            triangles.Add(baseIdx + 1);
            triangles.Add(baseIdx + 2);

            triangles.Add(baseIdx);
            triangles.Add(baseIdx + 2);
            triangles.Add(baseIdx + 3);
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetNormals(normals);
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}


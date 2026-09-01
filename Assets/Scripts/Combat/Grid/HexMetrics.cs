using UnityEngine;

public static class HexMetrics
{
    public const float outerRadius = 1.0f;
    public const float innerRadius = outerRadius * 0.866025404f; // sqrt(3)/2 ≈ 0.8660254

    // Vértices de un hexágono "Pointy-Topped" (con punta arriba)
    public static readonly Vector3[] corners = new Vector3[]
    {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius) // Repetir el primero para cerrar ciclos fácilmente
    };

    // Dirección de los 6 vecinos en coordenadas offset (Pointy-Top, fila par/impar)
    public static readonly Vector2Int[][] neighbors = new Vector2Int[][]
    {
        // Filas pares (z % 2 == 0)
        new Vector2Int[]
        {
            new Vector2Int(0, 1),   // NE
            new Vector2Int(1, 0),   // E
            new Vector2Int(0, -1),  // SE
            new Vector2Int(-1, -1), // SW
            new Vector2Int(-1, 0),  // W
            new Vector2Int(-1, 1)   // NW
        },
        // Filas impares (z % 2 != 0)
        new Vector2Int[]
        {
            new Vector2Int(1, 1),   // NE
            new Vector2Int(1, 0),   // E
            new Vector2Int(1, -1),  // SE
            new Vector2Int(0, -1),  // SW
            new Vector2Int(-1, 0),  // W
            new Vector2Int(0, 1)    // NW
        }
    };

    /// <summary>
    /// Calcula la posición local en el mundo para una coordenada de cuadrícula (columna, fila).
    /// </summary>
    public static Vector3 GetWorldPosition(int x, int z)
    {
        float posX = (x + (z & 1) * 0.5f) * (innerRadius * 2f);
        float posZ = z * (outerRadius * 1.5f);
        return new Vector3(posX, 0f, posZ);
    }
}


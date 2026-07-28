using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Tile tilePrefab;

    [SerializeField] private int width = 30;
    [SerializeField] private int height = 30;

    [SerializeField] private float cellSize = 1.05f;

    private Tile[,] tiles;
    
    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(
                    x * cellSize,
                    0,
                    y * cellSize
                );

                Tile tile = Instantiate(
                    tilePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                tile.X = x;
                tile.Y = y;

                tiles[x, y] = tile;
            }
        }
    }
}

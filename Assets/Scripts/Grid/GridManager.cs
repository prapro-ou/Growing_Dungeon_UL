using UnityEngine;
using Unity.AI.Navigation;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Tile tilePrefab;

    [SerializeField] private int width = 30;
    [SerializeField] private int height = 30;

    [SerializeField] private float cellSize = 1.05f;

    [Header("References")]
    [SerializeField] private Transform spawnPoint; // HierarchyのSpawnPointをセット
    [SerializeField] private Transform goalPoint;  // HierarchyのGoalPointをセット

    private Tile[,] tiles;
    private NavMeshSurface navMeshSurface;

    private void Awake()
    {
        // NavMeshSurface の参照をキャッシュしておきます
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    private void Start()
    {
        GenerateGrid();
        SetSpawnAndGoal(); // グリッド生成後に位置を合わせる
    }

    private void GenerateGrid()
    {
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // GridManager自体の位置(transform.position)を考慮したローカルオフセット計算
                Vector3 position = transform.position + new Vector3(
                    x * cellSize,
                    0, // 高さ（Y）は0で固定
                    y * cellSize // Z軸奥方向に並べる
                );

                Tile tile = Instantiate(
                    tilePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                tile.name = $"Tile_{x}_{y}";

                tile.X = x;
                tile.Y = y;

                tiles[x, y] = tile;
            }
        }

        // グリッド生成後にNavMeshを更新
        RebuildNavMesh();
    }

    private void SetSpawnAndGoal()
    {
        // 左下 (0, 0) のタイル位置に SpawnPoint を移動
        if (spawnPoint != null && tiles[0, 0] != null)
        {
            Tile spawnTile = tiles[0, 0];

            // タイルをSpawnに設定
            spawnTile.Type = TileType.Spawn;

            Vector3 pos = spawnTile.transform.position;
            pos.y += 0.5f; // 床から少し浮かす（元の高さに+0.5）
            spawnPoint.position = pos;
        }

        // 右上 (width-1, height-1) のタイル位置に GoalPoint を移動
        if (goalPoint != null && tiles[width - 1, height - 1] != null)
        {
            Vector3 pos = tiles[width - 1, height - 1].transform.position;
            pos.y += 0.5f; // ゴールの当たり判定も少し上に
            goalPoint.position = pos;
        }
    }

    /// <summary>
    /// NavMeshを再ビルド（再計算）する関数
    /// GameManagerやタワー配置時などから自由に呼び出せます
    /// </summary>
    public void RebuildNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
        }
        else
        {
            Debug.LogError("GridManager に NavMeshSurface コンポーネントがアタッチされていません！");
        }
    }

    /// <summary>
    /// 外部（マウスクリック等）から指定した(x, y)座標のTileを取得するための便利関数
    /// </summary>
    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return tiles[x, y];
        }
        return null;
    }
}
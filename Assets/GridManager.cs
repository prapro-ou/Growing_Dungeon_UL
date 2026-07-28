using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("グリッドの設定")]
    public int width = 10;   // 横幅（20に設定）
    public int height = 20;  // 縦の奥行き（10に設定）
    public float cellSize = 1.1f; // マスとマスの間隔

    [Header("生成する床のプレハブ")]
    public GameObject tilePrefab; 

    private int[,] gridData;

    void Start()
    {
        gridData = new int[width, height];
        GenerateGrid();
    }

    void GenerateGrid()
    {
        GameObject temporaryTemplate = null;

        // プレハブがない場合は、この関数内だけで使う仮のキューブを作成
        if (tilePrefab == null)
        {
            temporaryTemplate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            temporaryTemplate.transform.localScale = new Vector3(1f, 0.1f, 1f);
            tilePrefab = temporaryTemplate;
        }

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 spawnPosition = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject tile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);
                tile.transform.SetParent(this.transform);
                tile.name = $"Tile_{x}_{z}";

                gridData[x, z] = 0;
            }
        }

        // コピーが終わったら、型紙に使った仮のキューブだけを確実に消去する
        if (temporaryTemplate != null)
        {
            Destroy(temporaryTemplate);
        }
    }
}
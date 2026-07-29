using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class DungeonCore3D : MonoBehaviour
{
    [Header("システム設定")]
    public NavMeshSurface navMeshSurface; // 3D用のNavMeshSurface
    public GameObject wallPrefab;          // 配置する3Dの壁（Cubeなど）
    public LayerMask floorLayer;           // 床のレイヤー（Floor）

    [Header("キャラクター参照")]
    public Transform enemyTransform;       // 敵（Enemy2D）
    public Transform goalTransform;        // ゴール（Goal）

    private NavMeshAgent enemyAgent;

    void Start()
    {
        if (enemyTransform != null)
        {
            enemyAgent = enemyTransform.GetComponent<NavMeshAgent>();
            // 敵の初期目的地をゴールに設定
            SetEnemyDestination();
        }
        
        // 最初のナビゲーションマップを生成
        RebakeDungeon();
    }

    void Update()
    {
        // マウスの左クリックで壁を設置
        if (Input.GetMouseButtonDown(0))
        {
            PlaceWall();
        }
    }

void PlaceWall()
    {
        Debug.Log("クリックを検知しました！"); // ←追加

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, floorLayer))
        {
            Debug.Log("床（Floorレイヤー）への衝突を検知しました！座標: " + hit.point); // ←追加

            Vector3 snappedPos = new Vector3(
                Mathf.Round(hit.point.x),
                0.5f,
                Mathf.Round(hit.point.z)
            );

            if (wallPrefab != null)
            {
                Instantiate(wallPrefab, snappedPos, Quaternion.identity);
                Debug.Log("壁を生成しました！"); // ←追加
                
                RebakeDungeon();
                SetEnemyDestination();
            }
            else
            {
                Debug.LogWarning("Wall Prefabが登録されていません！"); // ←追加
            }
        }
    }

    void SetEnemyDestination()
    {
        if (enemyAgent != null && goalTransform != null)
        {
            enemyAgent.SetDestination(goalTransform.position);
        }
    }

    void RebakeDungeon()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh(); // マップを再スキャン
        }
    }
}
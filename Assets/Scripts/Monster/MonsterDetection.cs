using UnityEngine;

public class MonsterDetection : MonoBehaviour
{
    [Header("Layer")]
    [SerializeField] private LayerMask intruderLayer;
    [SerializeField] private LayerMask obstacleLayer;

    private Monster monster;

    private void Awake()
    {
        monster = GetComponent<Monster>();
    }

    /// <summary>
    /// 索敵範囲内にいて、壁に遮られていない侵入者を探す
    /// </summary>
    public IntruderNavMesh FindIntruder()
    {
        if (monster == null)
            return null;

        // Monsterから種類ごとの索敵範囲を取得
        float detectionRange = monster.DetectionRange;

        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            detectionRange,
            intruderLayer
        );

        foreach (Collider collider in colliders)
        {
            IntruderNavMesh intruder =
                collider.GetComponentInParent<IntruderNavMesh>();

            if (intruder == null)
                continue;

            Vector3 direction =
                intruder.transform.position - transform.position;

            float distance = direction.magnitude;

            // 壁に遮られているか確認
            if (Physics.Raycast(
                transform.position,
                direction.normalized,
                distance,
                obstacleLayer))
            {
                continue;
            }

            return intruder;
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (monster == null)
            monster = GetComponent<Monster>();

        if (monster == null)
            return;

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            transform.position,
            monster.DetectionRange
        );
    }
}
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("攻撃設定")]
    [Tooltip("攻撃範囲")]
    [SerializeField] private float attackRange = 1.5f;

    [Tooltip("攻撃間隔（秒）")]
    [SerializeField] private float attackInterval = 1.0f;

    private float attackTimer = 0f;
    private Monster monsterData;

    private void Awake()
    {
        // 同じオブジェクトに付いているMonsterを取得
        monsterData = GetComponent<Monster>();
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        // 一番近い敵を探す
        IntruderNavMesh target = FindNearestEnemy();

        if (target == null)
            return;

        // 敵との距離を確認
        float distance = Vector3.Distance(
            transform.position,
            target.transform.position
        );

        // 攻撃範囲内なら攻撃
        if (distance <= attackRange)
        {
            if (attackTimer >= attackInterval)
            {
                AttackTarget(target);
                attackTimer = 0f;
            }
        }
    }

    private IntruderNavMesh FindNearestEnemy()
    {
        IntruderNavMesh[] enemies =
            FindObjectsByType<IntruderNavMesh>(
                FindObjectsInactive.Exclude
            );

        IntruderNavMesh nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (IntruderNavMesh enemy in enemies)
        {
            if (enemy == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private void AttackTarget(IntruderNavMesh target)
    {
        if (target == null)
            return;

        int damage = monsterData != null
            ? monsterData.Attak
            : 20;

        target.TakeDamage(damage);
    }

    // エディタ上で攻撃範囲を表示
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}
using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour
{
    [Header("攻撃設定")]
    [Tooltip("攻撃範囲")]
    [SerializeField] private float attackRange = 1.5f;

    [Tooltip("攻撃間隔（秒）")]
    [SerializeField] private float attackInterval = 1.0f;

    private float attackTimer = 0f;
    private Monster monsterData;

    // 攻撃中か
    private bool isAttacking = false;

    private MonsterView monsterView;

    private void Awake()
    {
        monsterData = GetComponent<Monster>();
        monsterView = GetComponent<MonsterView>();
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
            if (attackTimer >= attackInterval && !isAttacking)
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
        if (target == null || isAttacking)
            return;

        StartCoroutine(AttackCoroutine(target));
    }

    private IEnumerator AttackCoroutine(IntruderNavMesh target)
    {
        isAttacking = true;

        // 攻撃対象が消えていたら終了
        if (target == null)
        {
            isAttacking = false;
            yield break;
        }

        // 攻撃対象の方向を向く
        if (monsterView != null)
        {
            monsterView.FaceTarget(
                target.transform.position
            );

            // 攻撃モーション
            yield return StartCoroutine(
                monsterView.PlayAttack(
                    target.transform.position
                )
            );
        }
        else
        {
            // MonsterViewがない場合
            yield return new WaitForSeconds(0.1f);
        }

        // モーション終了後にダメージ
        if (target != null)
        {
            int damage = monsterData != null
                ? monsterData.Attak
                : 20;

            target.TakeDamage(damage);
        }

        isAttacking = false;
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
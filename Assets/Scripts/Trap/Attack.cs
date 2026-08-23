using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    private Monster monsterData;
    private MonsterView monsterView;

    private float attackTimer = 0f;

    // 現在の攻撃対象
    private IntruderNavMesh target;

    // 攻撃中か
    private bool isAttacking = false;

    private void Awake()
    {
        monsterData = GetComponent<Monster>();
        monsterView = GetComponent<MonsterView>();
    }

    private void Update()
    {
        if (monsterData == null)
            return;

        if (target == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            target.transform.position
        );

        // 攻撃範囲外なら何もしない
        if (distance > monsterData.AttackRange)
            return;

        attackTimer += Time.deltaTime;

        // MonsterDataの攻撃間隔を使用
        if (attackTimer >= monsterData.AttackInterval &&
            !isAttacking)
        {
            AttackTarget();
            attackTimer = 0f;
        }
    }

    // =========================
    // 攻撃対象を設定
    // =========================

    public void SetTarget(IntruderNavMesh newTarget)
    {
        target = newTarget;
    }

    // =========================
    // 攻撃対象を解除
    // =========================

    public void ClearTarget()
    {
        target = null;
    }

    // =========================
    // 攻撃開始
    // =========================

    private void AttackTarget()
    {
        if (target == null || isAttacking)
            return;

        StartCoroutine(AttackCoroutine());
    }

    // =========================
    // 攻撃モーション
    // =========================

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        IntruderNavMesh attackTarget = target;

        if (attackTarget == null)
        {
            isAttacking = false;
            yield break;
        }

        // 攻撃対象の方向を向く
        if (monsterView != null)
        {
            monsterView.FaceTarget(
                attackTarget.transform.position
            );

            // 攻撃モーション
            yield return StartCoroutine(
                monsterView.PlayAttack(
                    attackTarget.transform.position
                )
            );
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
        }

        // モーション中に対象が消えていなければダメージ
        if (attackTarget != null)
        {
            attackTarget.TakeDamage(
                monsterData.Attack
            );
        }

        isAttacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (monsterData == null)
            monsterData = GetComponent<Monster>();

        if (monsterData == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            monsterData.AttackRange
        );
    }
}
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("攻撃設定")]
    [Tooltip("攻撃間隔（秒）")]
    [SerializeField] private float attackInterval = 1.0f;

    private float attackTimer = 0f;
    private Monster monsterData;
    private List<IntruderNavMesh> enemiesInRange = new List<IntruderNavMesh>();

    private void Awake()
    {
        // 同じオブジェクトに付いている Monster スクリプトを取得
        monsterData = GetComponent<Monster>();
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;

        // 撃破されて消えた敵（null）をリストから除去
        enemiesInRange.RemoveAll(enemy => enemy == null);

        // 範囲内に敵がいて、攻撃準備完了なら攻撃
        if (attackTimer >= attackInterval && enemiesInRange.Count > 0)
        {
            AttackTarget(enemiesInRange[0]);
            attackTimer = 0f;
        }
    }

    private void AttackTarget(IntruderNavMesh target)
    {
        if (target == null) return;

        // Monsterスクリプトの Attak 値を使用（設定がなければデフォルト20）
        int damage = (monsterData != null) ? monsterData.Attak : 20;
        target.TakeDamage(damage);
    }

    // 2D攻撃範囲（CircleCollider2D: Is Trigger）に敵が入った時
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IntruderNavMesh enemy = other.GetComponent<IntruderNavMesh>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    // 2D攻撃範囲から敵が出た時
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            IntruderNavMesh enemy = other.GetComponent<IntruderNavMesh>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }

    // エディタ上で攻撃範囲を赤線で可視化
    private void OnDrawGizmosSelected()
    {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + (Vector3)col.offset, col.radius);
        }
    }
}
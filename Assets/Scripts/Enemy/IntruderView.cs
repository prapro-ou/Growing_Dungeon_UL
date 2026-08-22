using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class IntruderView : MonoBehaviour
{
    [SerializeField] private Transform front;

    private NavMeshAgent agent;

    [Header("攻撃モーション")]
    [SerializeField] private float attackDistance = 0.3f;
    [SerializeField] private float attackBackTime = 0.08f;
    [SerializeField] private float attackForwardTime = 0.08f;
    [SerializeField] private float attackReturnTime = 0.12f;

    private Vector3 frontOriginalPosition;

    [Header("ダメージ演出")]
    [SerializeField] private float damageFlashTime = 0.1f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine damageFlashCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (front != null)
        {
            frontOriginalPosition = front.localPosition;
            spriteRenderer = front.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
        }
    }

    private void Update()
    {
        if (agent != null && !agent.isStopped)
        {
            UpdateDirection(agent.velocity);
        }
    }

    // =========================
    // 移動方向を向く
    // =========================
    public void UpdateDirection(Vector3 velocity)
    {
        if (front == null)
            return;

        if (Mathf.Abs(velocity.x) < 0.01f)
            return;

        Vector3 scale = front.localScale;

        if (velocity.x > 0)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);

        front.localScale = scale;
    }

    // =========================
    // 攻撃対象の方向を向く
    // =========================
    public void FaceTarget(Vector3 targetPosition)
    {
        if (front == null)
            return;

        Vector3 direction =
            targetPosition - transform.position;

        // X方向が大きい場合だけ左右反転
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            Vector3 scale = front.localScale;

            if (direction.x > 0)
                scale.x = Mathf.Abs(scale.x);
            else
                scale.x = -Mathf.Abs(scale.x);

            front.localScale = scale;
        }
    }

    // =========================
    // 攻撃モーション
    // =========================
    public IEnumerator PlayAttack(Vector3 targetPosition)
    {
        if (front == null)
            yield break;

        // 現在位置を基準にする
        Vector3 startPosition = frontOriginalPosition;

        Vector3 direction =
            targetPosition - transform.position;

        // Y方向は使わない
        direction.y = 0f;

        Vector3 attackDirection;

        // =========================
        // 左右
        // =========================
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            attackDirection = new Vector3(
                Mathf.Sign(direction.x),
                0f,
                0f
            );

            // 左右の場合はスプライトを反転
            Vector3 scale = front.localScale;

            if (direction.x > 0)
                scale.x = Mathf.Abs(scale.x);
            else
                scale.x = -Mathf.Abs(scale.x);

            front.localScale = scale;
        }
        // =========================
        // 上下
        // =========================
        else
        {
            attackDirection = new Vector3(
                0f,
                0f,
                Mathf.Sign(direction.z)
            );
        }

        // =========================
        // 後ろに引く
        // =========================

        Vector3 backPosition =
            startPosition -
            attackDirection * 0.12f;

        float timer = 0f;

        while (timer < attackBackTime)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(timer / attackBackTime);

            front.localPosition =
                Vector3.Lerp(
                    startPosition,
                    backPosition,
                    t
                );

            yield return null;
        }

        // =========================
        // 攻撃方向へ突き出す
        // =========================

        Vector3 attackPosition =
            startPosition +
            attackDirection * attackDistance;

        timer = 0f;

        while (timer < attackForwardTime)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(timer / attackForwardTime);

            front.localPosition =
                Vector3.Lerp(
                    backPosition,
                    attackPosition,
                    t
                );

            yield return null;
        }

        // 攻撃が当たった瞬間
        yield return new WaitForSeconds(0.05f);

        // =========================
        // 元の位置へ戻す
        // =========================

        timer = 0f;

        while (timer < attackReturnTime)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(timer / attackReturnTime);

            front.localPosition =
                Vector3.Lerp(
                    attackPosition,
                    startPosition,
                    t
                );

            yield return null;
        }

        front.localPosition = startPosition;
    }

    public void PlayDamageFlash()
    {
        if (spriteRenderer == null)
            return;

        if (damageFlashCoroutine != null)
        {
            StopCoroutine(damageFlashCoroutine);
        }

        damageFlashCoroutine = StartCoroutine(DamageFlashCoroutine());
    }

    private IEnumerator DamageFlashCoroutine()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(damageFlashTime);

        spriteRenderer.color = originalColor;

        damageFlashCoroutine = null;
    }
}
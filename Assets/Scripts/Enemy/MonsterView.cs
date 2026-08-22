using System.Collections;
using UnityEngine;

public class MonsterView : MonoBehaviour
{
    [SerializeField] private GameObject top;
    [SerializeField] private GameObject front;

    private WaveManager waveManager;

    [Header("攻撃モーション")]
    [SerializeField] private float attackDistance = 0.3f;
    [SerializeField] private float attackBackTime = 0.08f;
    [SerializeField] private float attackForwardTime = 0.08f;
    [SerializeField] private float attackReturnTime = 0.12f;

    private Vector3 frontOriginalPosition;

    private void Start()
    {
        if (front != null)
        {
            frontOriginalPosition = front.transform.localPosition;
        }

        waveManager = FindAnyObjectByType<WaveManager>();

        if (waveManager != null)
        {
            waveManager.onPhaseChanged += ChangePhase;
            ChangePhase(waveManager.currentPhase);
        }
    }

    private void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.onPhaseChanged -= ChangePhase;
        }
    }

    private void ChangePhase(WaveManager.GamePhase phase)
    {
        if (phase == WaveManager.GamePhase.PrepPhase)
        {
            top.SetActive(true);
            front.SetActive(false);
        }
        else if (phase == WaveManager.GamePhase.WavePhase)
        {
            top.SetActive(false);
            front.SetActive(true);
        }
    }

    public void FaceTarget(Vector3 targetPosition)
    {
        if (front == null)
            return;

        float direction = targetPosition.x - transform.position.x;

        if (Mathf.Abs(direction) < 0.01f)
            return;

        Vector3 scale = front.transform.localScale;

        if (direction > 0)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);

        front.transform.localScale = scale;
    }

    public IEnumerator PlayAttack(Vector3 targetPosition)
    {
        if (front == null)
            yield break;

        FaceTarget(targetPosition);

        Vector3 startPosition = frontOriginalPosition;

        // 攻撃対象への方向を計算
        Vector3 worldDirection = targetPosition - transform.position;

        // Y方向は無視してX・Zだけで方向を決める
        worldDirection.y = 0f;

        if (worldDirection.sqrMagnitude < 0.001f)
            yield break;

        worldDirection.Normalize();

        // ワールド方向をMonsterのローカル方向に変換
        Vector3 localDirection =
            transform.InverseTransformDirection(worldDirection);

        localDirection.y = 0f;
        localDirection.Normalize();

        // =========================
        // 少し後ろに引く
        // =========================

        Vector3 backPosition =
            startPosition - localDirection * 0.12f;

        float timer = 0f;

        while (timer < attackBackTime)
        {
            timer += Time.deltaTime;

            float t = timer / attackBackTime;

            front.transform.localPosition =
                Vector3.Lerp(startPosition, backPosition, t);

            yield return null;
        }

        // =========================
        // 攻撃方向へ踏み込む
        // =========================

        Vector3 attackPosition =
            startPosition + localDirection * attackDistance;

        timer = 0f;

        while (timer < attackForwardTime)
        {
            timer += Time.deltaTime;

            float t = timer / attackForwardTime;

            front.transform.localPosition =
                Vector3.Lerp(backPosition, attackPosition, t);

            yield return null;
        }

        // 少し止める
        yield return new WaitForSeconds(0.05f);

        // =========================
        // 元の位置に戻る
        // =========================

        timer = 0f;

        while (timer < attackReturnTime)
        {
            timer += Time.deltaTime;

            float t = timer / attackReturnTime;

            front.transform.localPosition =
                Vector3.Lerp(attackPosition, startPosition, t);

            yield return null;
        }

        front.transform.localPosition = startPosition;
    }
}
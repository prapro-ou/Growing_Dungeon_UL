using System.Collections;
using UnityEngine;

public class MonsterView : MonoBehaviour
{
    [SerializeField] private GameObject top;
    [SerializeField] private GameObject front;

    private Renderer[] topRenderers;
    private Color[] topOriginalColors;

    private WaveManager waveManager;
    private PlacedObjectInfo placedObjectInfo;

    [Header("建築モード表示")]
    [SerializeField] private float previousWaveAlpha = 0.9f;

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

        if (top != null)
        {
            topRenderers = top.GetComponentsInChildren<Renderer>(true);
            topOriginalColors = new Color[topRenderers.Length];

            for (int i = 0; i < topRenderers.Length; i++)
            {
                Material material = topRenderers[i].material;

                if (material.HasProperty("_BaseColor"))
                {
                    topOriginalColors[i] = material.GetColor("_BaseColor");
                }
                else if (material.HasProperty("_Color"))
                {
                    topOriginalColors[i] = material.GetColor("_Color");
                }
            }
        }

        // 設置情報を取得
        placedObjectInfo = GetComponent<PlacedObjectInfo>();

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

            float alpha = 1f;

            // 前WAVEで設置されたモンスターだけ半透明
            if (placedObjectInfo != null &&
                waveManager != null &&
                placedObjectInfo.PlacedWave < waveManager.currentWaveIndex)
            {
                alpha = previousWaveAlpha;
            }

            SetTopTransparency(alpha);
        }
        else if (phase == WaveManager.GamePhase.WavePhase)
        {
            top.SetActive(false);
            front.SetActive(true);

            // 侵略モードでは完全に通常表示
            SetFrontTransparency(1f);
        }
    }

    private void SetTopTransparency(float alpha)
    {
        if (top == null)
            return;

        Renderer[] renderers =
            top.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material material = renderer.material;

            Color color = material.color;
            color.a = alpha;
            material.color = color;
        }
    }

    private void SetFrontTransparency(float alpha)
    {
        if (front == null)
            return;

        Renderer[] renderers =
            front.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            Material material = renderer.material;

            Color color = material.color;
            color.a = alpha;
            material.color = color;
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

    public void UpdateDirection(Vector3 velocity)
    {
        if (front == null)
            return;

        if (Mathf.Abs(velocity.x) < 0.01f)
            return;

        Vector3 scale = front.transform.localScale;

        if (velocity.x > 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else if (velocity.x < 0)
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        front.transform.localScale = scale;
    }

    public IEnumerator PlayAttack(Vector3 targetPosition)
    {
        if (front == null)
            yield break;

        FaceTarget(targetPosition);

        Vector3 startPosition = frontOriginalPosition;

        Vector3 worldDirection = targetPosition - transform.position;

        worldDirection.y = 0f;

        if (worldDirection.sqrMagnitude < 0.001f)
            yield break;

        worldDirection.Normalize();

        Vector3 localDirection =
            transform.InverseTransformDirection(worldDirection);

        localDirection.y = 0f;
        localDirection.Normalize();

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

        yield return new WaitForSeconds(0.05f);

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
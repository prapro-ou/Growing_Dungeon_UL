using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    [Header("Wall")]
    [SerializeField] private GameObject wallPreviewPrefab;

    [Header("Monster")]
    [SerializeField] private GameObject[] monsterPreviewPrefabs;

    [Header("Treasure")]
    [SerializeField] private GameObject[] treasurePreviewPrefabs;


    private GameObject wallPreview;
    private GameObject currentPreview;

    private MonsterType currentMonsterType = MonsterType.None;
    private TreasureType currentTreasureType = TreasureType.None;


    private void Start()
    {
        // 壁プレビューだけ最初に生成
        if (wallPreviewPrefab != null)
        {
            wallPreview = Instantiate(wallPreviewPrefab, transform);
            wallPreview.SetActive(false);
        }
    }


    // =========================
    // プレビュー移動
    // =========================

    public void MovePreview(Vector3 position)
    {
        if (currentPreview == null)
            return;

        currentPreview.SetActive(true);
        currentPreview.transform.position = position;
    }


    // =========================
    // 非表示
    // =========================

    public void HidePreview()
    {
        if (currentPreview == null)
            return;

        currentPreview.SetActive(false);
    }


    // =========================
    // 全消去
    // =========================

    public void ClearPreview()
    {
        if (wallPreview != null)
            wallPreview.SetActive(false);

        if (currentPreview != null && currentPreview != wallPreview)
        {
            Destroy(currentPreview);
        }

        currentPreview = null;
    }


    // =========================
    // BuildMode変更
    // =========================

    public void SetPreview(BuildMode mode)
    {
        ClearPreview();

        switch (mode)
        {
            case BuildMode.Wall:
                SetWallPreview();
                break;

            case BuildMode.Monster:
                if (currentMonsterType != MonsterType.None)
                    SetMonsterPreview(currentMonsterType);
                break;

            case BuildMode.Treasure:
                if (currentTreasureType != TreasureType.None)
                    SetTreasurePreview(currentTreasureType);
                break;

            default:
                break;
        }
    }


    // =========================
    // Wall
    // =========================

    public void SetWallPreview()
    {
        if (wallPreview == null)
            return;

        if (currentPreview != null && currentPreview != wallPreview)
            currentPreview.SetActive(false);

        currentPreview = wallPreview;

        SetPreviewColor(currentPreview);

        currentPreview.SetActive(true);
    }


    // =========================
    // Monster
    // =========================

    public void SetMonsterPreview(MonsterType type)
    {
        ClearPreview();

        currentMonsterType = type;

        if (type == MonsterType.None)
            return;

        int index = (int)type - 1;

        if (index < 0 || index >= monsterPreviewPrefabs.Length)
        {
            Debug.LogWarning("対応するMonster Previewがありません: " + type);
            return;
        }

        if (monsterPreviewPrefabs[index] == null)
        {
            Debug.LogWarning("Monster Preview Prefabが未設定です: " + type);
            return;
        }

        currentPreview = Instantiate(
            monsterPreviewPrefabs[index],
            transform
        );

        SetPreviewColor(currentPreview);

        currentPreview.SetActive(true);
    }


    // =========================
    // Treasure
    // =========================

    public void SetTreasurePreview(TreasureType type)
    {
        ClearPreview();

        currentTreasureType = type;

        if (type == TreasureType.None)
            return;

        int index = (int)type - 1;

        if (index < 0 || index >= treasurePreviewPrefabs.Length)
        {
            Debug.LogWarning("対応するTreasure Previewがありません: " + type);
            return;
        }

        if (treasurePreviewPrefabs[index] == null)
        {
            Debug.LogWarning("Treasure Preview Prefabが未設定です: " + type);
            return;
        }

        currentPreview = Instantiate(
            treasurePreviewPrefabs[index],
            transform
        );

        SetPreviewColor(currentPreview);

        currentPreview.SetActive(true);
    }

    private void SetPreviewColor(GameObject preview)
    {
        if (preview == null)
            return;

        Renderer[] renderers =
            preview.GetComponentsInChildren<Renderer>(true);

        Color previewColor = new Color(0f, 1f, 1f, 0.5f);

        foreach (Renderer renderer in renderers)
        {
            Material material = renderer.material;

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", previewColor);
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", previewColor);
            }
        }
    }

    public void SetPreviewValid(bool valid)
    {
        if (currentPreview == null)
            return;

        Renderer[] renderers =
            currentPreview.GetComponentsInChildren<Renderer>(true);

        Color color = valid
            ? new Color(0f, 1f, 1f, 0.7f)   // 水色
            : new Color(1f, 0f, 0f, 0.7f);   // 赤

        foreach (Renderer renderer in renderers)
        {
            Material material = renderer.material;

            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }
            else if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", color);
            }
        }
    }
}
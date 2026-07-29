using UnityEngine; // 不要な System.Runtime.InteropServices は削除しています

public class PreviewManager : MonoBehaviour
{
    [SerializeField]
    private GameObject wallPreviewPrefab;

    [SerializeField]
    private GameObject monsterPreviewPrefab;

    private Renderer previewRenderer;

    private GameObject wallPreview;
    private GameObject monsterPreview;

    private GameObject currentPreview;

    private void Start()
    {
        if (wallPreviewPrefab != null)
        {
            wallPreview = Instantiate(wallPreviewPrefab, transform);
            wallPreview.SetActive(false);
        }

        if (monsterPreviewPrefab != null)
        {
            monsterPreview = Instantiate(monsterPreviewPrefab, transform);
            monsterPreview.SetActive(false);
        }
    }

    public void MovePreview(Vector3 position)
    {
        if (currentPreview == null)
            return;

        currentPreview.SetActive(true);
        currentPreview.transform.position = position;
    }

    public void HidePreview()
    {
        if (currentPreview == null)
            return;

        currentPreview.SetActive(false);
    }

    /// <summary>
    /// すべてのプレビューを非表示にし、参照をリセットする（BuildMenuUIのClearPreview呼び出しに対応）
    /// </summary>
    public void ClearPreview()
    {
        if (wallPreview != null)
            wallPreview.SetActive(false);

        if (monsterPreview != null)
            monsterPreview.SetActive(false);

        currentPreview = null;
        previewRenderer = null;
    }

    public void SetPreview(BuildMode mode)
    {
        if (wallPreview != null)
            wallPreview.SetActive(false);

        if (monsterPreview != null)
            monsterPreview.SetActive(false);

        switch (mode)
        {
            case BuildMode.Wall:
                currentPreview = wallPreview;
                if (wallPreview != null)
                    previewRenderer = wallPreview.GetComponentInChildren<Renderer>();
                break;

            case BuildMode.Monster:
                currentPreview = monsterPreview;
                if (monsterPreview != null)
                    previewRenderer = monsterPreview.GetComponentInChildren<Renderer>();
                break;

            default:
                currentPreview = null;
                previewRenderer = null;
                break;
        }
    }
}
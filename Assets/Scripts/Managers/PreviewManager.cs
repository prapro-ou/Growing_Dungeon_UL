using System.Runtime.InteropServices;
using UnityEngine;

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
        wallPreview = Instantiate(wallPreviewPrefab, transform);
        wallPreview.SetActive(false);

        monsterPreview = Instantiate(monsterPreviewPrefab, transform);
        monsterPreview.SetActive(false);
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
                previewRenderer = wallPreview.GetComponentInChildren<MeshRenderer>();
                break;

            case BuildMode.Monster:
                currentPreview = monsterPreview;
                previewRenderer = monsterPreview.GetComponentInChildren<SpriteRenderer>();
                break;

            default:
                currentPreview = null;
                previewRenderer = null;
                break;
        }
        
    }
}
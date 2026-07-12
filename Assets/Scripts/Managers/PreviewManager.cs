using System.Runtime.InteropServices;
using UnityEngine;

public class PreviewManager : MonoBehaviour
{
    [SerializeField]
    private GameObject wallPreviewPrefab;

    private Renderer previewRenderer;

    private GameObject currentPreview;

    private void Start()
    {
        currentPreview = Instantiate(wallPreviewPrefab, transform);
        currentPreview.SetActive(false);

        previewRenderer = currentPreview.GetComponent<MeshRenderer>();
    }

    public void MovePreview(Vector3 position)
    {
        currentPreview.SetActive(true);

        position.y += 0.25f;

        currentPreview.transform.position = position;
    }

    public void HidePreview()
    {
        currentPreview.SetActive(false);
    }

    public void SetCanPlace(bool canPlace)
    {
        Color color = canPlace ? Color.cyan : Color.red;

        color.a = 0.5f;

        previewRenderer.material.color = color;
    }
}
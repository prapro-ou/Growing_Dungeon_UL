using UnityEngine;
using UnityEngine.UI;

public class TreasureMenuUI : MonoBehaviour
{
    [SerializeField] private ChangeMenuUI changeMenuUI;

    [Header("Manager")]
    [SerializeField] private BuildManager buildManager;
    [SerializeField] private PreviewManager previewManager;

    [Header("Treasure Buttons")]
    [SerializeField] private Button MainTreasureButton;
    [SerializeField] private Button SubTreasureButton;
    [SerializeField] private Button TreasureEraseButton;

    [Header("Button Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.3f, 0.7f, 1f);

    private void OnEnable()
    {
        UpdateButtonColors();
    }

    public void MainTreasure()
    {
        buildManager.SetTreasureType(TreasureType.MainTreasure);
        buildManager.SetBuildMode(BuildMode.Treasure);

        previewManager.SetTreasurePreview(TreasureType.MainTreasure);

        UpdateButtonColors();
    }

    public void SubTreasure()
    {
        buildManager.SetTreasureType(TreasureType.SubTreasure);
        buildManager.SetBuildMode(BuildMode.Treasure);

        previewManager.SetTreasurePreview(TreasureType.SubTreasure);

        UpdateButtonColors();
    }

    public void Erase()
    {
        buildManager.SetTreasureType(TreasureType.None);
        buildManager.SetBuildMode(BuildMode.Erase);

        previewManager.ClearPreview();

        UpdateButtonColors();
    }

    private void UpdateButtonColors()
    {
        SetButtonColor(MainTreasureButton, TreasureType.MainTreasure);
        SetButtonColor(SubTreasureButton, TreasureType.SubTreasure);
        SetButtonColor(TreasureEraseButton, TreasureType.None);
    }

    private void SetButtonColor(Button button, TreasureType type)
    {
        if (button == null) return;

        button.image.color =
            (buildManager != null &&
             buildManager.CurrentTreasureType == type)
            ? selectedColor
            : normalColor;
    }
}
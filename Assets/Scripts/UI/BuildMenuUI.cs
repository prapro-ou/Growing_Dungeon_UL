using UnityEngine;
using UnityEngine.UI;

public class BuildMenuUI : MonoBehaviour
{
    [SerializeField] private BuildManager buildManager;

    [Header("Buttons")]
    [SerializeField] private Button wallButton;
    [SerializeField] private Button trapButton;
    [SerializeField] private Button monsterButton;
    [SerializeField] private Button treasureButton;
    [SerializeField] private Button eraseButton;

    [Header("Button Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.3f, 0.7f, 1f);

    private void Start()
    {
        UpdateButtonColors();
    }

    public void Wall()
    {
        buildManager.SetBuildMode(BuildMode.Wall);
        UpdateButtonColors();
    }

    public void Trap()
    {
        buildManager.SetBuildMode(BuildMode.Trap);
        UpdateButtonColors();
    }

    public void Monster()
    {
        buildManager.SetBuildMode(BuildMode.Monster);
        UpdateButtonColors();
    }

    public void Treasure()
    {
        buildManager.SetBuildMode(BuildMode.Treasure);
        UpdateButtonColors();
    }

    public void Erase()
    {
        buildManager.SetBuildMode(BuildMode.Erase);
        UpdateButtonColors();
    }

    private void UpdateButtonColors()
    {
        SetButtonColor(wallButton, BuildMode.Wall);
        SetButtonColor(trapButton, BuildMode.Trap);
        SetButtonColor(monsterButton, BuildMode.Monster);
        SetButtonColor(treasureButton, BuildMode.Treasure);
        SetButtonColor(eraseButton, BuildMode.Erase);
    }

    private void SetButtonColor(Button button, BuildMode mode)
    {
        if (button == null)
            return;

        button.image.color =
            buildManager.CurrentMode == mode
            ? selectedColor
            : normalColor;
    }
}
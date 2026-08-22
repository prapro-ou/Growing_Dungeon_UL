using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildMenuUI : MonoBehaviour
{
    [SerializeField] private ChangeMenuUI changeMenuUI;

    [Header("Manager")]
    [SerializeField] private BuildManager buildManager;

    [Header("Build Buttons")]
    [SerializeField] private Button wallButton;
    [SerializeField] private Button trapButton;
    [SerializeField] private Button monsterButton;
    [SerializeField] private Button eraseButton;

    [Header("Button Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.3f, 0.7f, 1f);


    private void Start()
    {
        UpdateButtonColors();
    }

    private void OnEnable()
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

        changeMenuUI.ShowMonsterMenu();
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
        SetButtonColor(eraseButton, BuildMode.Erase);
    }

    private void SetButtonColor(Button button, BuildMode mode)
    {
        if (button == null) return;

        button.image.color =
            (buildManager != null && buildManager.CurrentMode == mode)
            ? selectedColor
            : normalColor;
    }
}
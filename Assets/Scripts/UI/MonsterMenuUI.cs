using UnityEngine;
using UnityEngine.UI;

public class MonsterMenuUI : MonoBehaviour
{
    [SerializeField] private ChangeMenuUI changeMenuUI;

    [Header("Manager")]
    [SerializeField] private BuildManager buildManager;

    [Header("Monster Buttons")]
    [SerializeField] private Button spiderButton;
    [SerializeField] private Button GoblinButton;
    [SerializeField] private Button GargoyleButton;
    [SerializeField] private Button SkeletonButton;
    [SerializeField] private Button DaemonButton;
    [SerializeField] private Button GolemButton;

    [Header("Button Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.3f, 0.7f, 1f);

    private void OnEnable()
    {
        UpdateButtonColors();
    }

    public void Spider()
    {
        buildManager.SetMonsterType(MonsterType.Spider);
        UpdateButtonColors();
    }

    public void Goblin()
    {
        buildManager.SetMonsterType(MonsterType.Goblin);
        UpdateButtonColors();
    }

    public void Gargoyle()
    {
        buildManager.SetMonsterType(MonsterType.Gargoyle);
        UpdateButtonColors();
    }

    public void Skeleton()
    {
        buildManager.SetMonsterType(MonsterType.Skeleton);
        UpdateButtonColors();
    }

    public void Daemon()
    {
        buildManager.SetMonsterType(MonsterType.Daemon);
        UpdateButtonColors();
    }

    public void Golem()
    {
        buildManager.SetMonsterType(MonsterType.Golem);
        UpdateButtonColors();
    }

    public void Return()
    {
        buildManager.SetMonsterType(MonsterType.None);
        changeMenuUI.ShowBuildMenu();
    }

    private void UpdateButtonColors()
    {
        SetButtonColor(spiderButton, MonsterType.Spider);
        SetButtonColor(GoblinButton, MonsterType.Goblin);
        SetButtonColor(GargoyleButton, MonsterType.Gargoyle);
        SetButtonColor(SkeletonButton, MonsterType.Skeleton);
        SetButtonColor(DaemonButton, MonsterType.Daemon);
        SetButtonColor(GolemButton, MonsterType.Golem);
    }

    private void SetButtonColor(Button button, MonsterType type)
    {
        if (button == null) return;

        button.image.color =
            (buildManager != null && buildManager.CurrentMonsterType == type)
            ? selectedColor
            : normalColor;
    }
}

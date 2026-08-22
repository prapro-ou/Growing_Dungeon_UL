using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HoverHPUI : MonoBehaviour
{
    [SerializeField] private GameObject hpPanel;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Camera mainCamera;

    [Header("HPバーの色")]
    [SerializeField] private Color highHPColor = Color.green;
    [SerializeField] private Color middleHPColor = Color.yellow;
    [SerializeField] private Color lowHPColor = Color.red;

    [Header("表示位置")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, 0f);

    [SerializeField] private Image hpPanelImage;

    private void Start()
    {
        Hide();
    }

    public void ShowMonster(Monster monster)
    {
        if (monster == null)
        {
            Hide();
            return;
        }

        hpPanel.SetActive(true);
        hpText.text = $"HP {monster.HP}/{monster.MaxHP}";

        UpdateHPColor(monster.HP, monster.MaxHP);

        SetPosition(monster.transform.position);
    }

    public void ShowTreasure(Treasure treasure)
    {
        if (treasure == null)
        {
            Hide();
            return;
        }

        hpPanel.SetActive(true);
        hpText.text = $"HP {treasure.currentHP}/{treasure.maxHP}";

        UpdateHPColor(treasure.currentHP, treasure.maxHP);

        SetPosition(treasure.transform.position);
    }

    private void UpdateHPColor(int currentHP, int maxHP)
    {
        float hpRatio = (float)currentHP / maxHP;

        if (hpRatio <= 0.2f)
        {
            hpPanelImage.color = lowHPColor;
        }
        else if (hpRatio <= 0.5f)
        {
            hpPanelImage.color = middleHPColor;
        }
        else
        {
            hpPanelImage.color = highHPColor;
        }
    }

    private void SetPosition(Vector3 worldPosition)
    {
        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(worldPosition + offset);

        hpPanel.transform.position = screenPosition;
    }

    public void Hide()
    {
        hpPanel.SetActive(false);
    }
}
using TMPro;
using UnityEngine;

public class DungeonPointUI : MonoBehaviour
{
    [SerializeField] private DungeonPointManager dungeonPointManager;
    [SerializeField] private TMP_Text dpText;

    private void Start()
    {
        dungeonPointManager.OnDPChanged += UpdateDPText;

        // 最初の表示
        UpdateDPText(dungeonPointManager.CurrentDP);
    }

    private void OnDestroy()
    {
        dungeonPointManager.OnDPChanged -= UpdateDPText;
    }

    private void UpdateDPText(int currentDP)
    {
        dpText.text = $"DP: {currentDP}";
    }
}

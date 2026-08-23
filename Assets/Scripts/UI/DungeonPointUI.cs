using TMPro;
using UnityEngine;

public class DungeonPointUI : MonoBehaviour
{
    [SerializeField] private DungeonPointManager dungeonPointManager;
    [SerializeField] private TMP_Text dpText;

    private void Start()
    {
        if (dungeonPointManager == null)
        {
            dungeonPointManager = FindFirstObjectByType<DungeonPointManager>();
        }

        if (dungeonPointManager != null)
        {
            dungeonPointManager.OnDPChanged += UpdateDPText;
            UpdateDPText(dungeonPointManager.CurrentDP);
        }
        else if (dpText != null)
        {
            dpText.text = "DP: 0";
        }
    }

    private void OnDestroy()
    {
        if (dungeonPointManager != null)
        {
            dungeonPointManager.OnDPChanged -= UpdateDPText;
        }
    }

    private void UpdateDPText(int currentDP)
    {
        if (dpText != null)
        {
            dpText.text = $"DP: {currentDP}";
        }
    }
}
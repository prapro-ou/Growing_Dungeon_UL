using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterButton : MonoBehaviour
{
    [Header("モンスターデータ")]
    public MonsterData monsterData;

    [Header("このボタンのモンスター")]
    public MonsterData.MonsterType monsterType;

    [Header("表示")]
    public TMP_Text pointText;

    [Header("ボタン")]
    public Button button;

    private void Start()
    {
        // モンスターのデータを取得
        MonsterData.MonsterStatus status =
            monsterData.GetStatus(monsterType);

        // 必要ポイントを表示
        pointText.text = status.buildCost + "P";
    }
}
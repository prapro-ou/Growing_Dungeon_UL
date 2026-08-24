using UnityEngine;
using TMPro;

public class WallButton : MonoBehaviour
{
    [Header("壁")]
    public Wall wall;

    [Header("必要ポイント表示")]
    public TMP_Text pointText;

    private void Start()
    {
        pointText.text = wall.BuildCost + "P";
    }
}
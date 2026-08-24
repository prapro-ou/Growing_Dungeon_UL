using UnityEngine;
using TMPro;

public class GameStartManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject titlePanel;    // TitlePanelを入れる
    public GameObject storyPanel;    // StoryPanelを入れる
    public GameObject buildMenu;     // Canvas内にあるBuildMenuを入れる
    public GameObject treasureMenu;

    [Header("DP Settings")] 
    public GameObject dpTextObject;

    [Header("Story Settings")]
    public TextMeshProUGUI storyText; // セリフ用のText(TMP)を入れる
    
    [TextArea(3, 5)]
    public string[] storyLines;      // 英語のストーリー文章を入れる

    private int currentLineIndex = 0;

    void Start()
    {
        // 起動時の初期化
        titlePanel.SetActive(true);
        storyPanel.SetActive(false);

        if (dpTextObject != null)
        {
            dpTextObject.SetActive(false);
        }
        
        // ゲーム開始前は本編のBuildMenuを非表示にしておく
        if (buildMenu != null)
        {
            buildMenu.SetActive(false);
        }
        if (treasureMenu != null)
        {
            treasureMenu.SetActive(false);
        }

        if (BGMManager.Instance != null)
        {
        BGMManager.Instance.PlayTitleBGM();
        }
    }

    // 「GAME START」ボタンを押した時
    public void OnClickStartButton()
    {
        titlePanel.SetActive(false);
        storyPanel.SetActive(true);
        buildMenu.SetActive(false);
        currentLineIndex = 0;
        ShowCurrentLine();
    }

    // ストーリー画面をクリックして次に進む時
    public void OnClickNextStory()
    {
        currentLineIndex++;

        if (currentLineIndex < storyLines.Length)
        {
            ShowCurrentLine();
        }
        else
        {
            // ストーリー終了！ゲーム本編スタート
            StartGameplay();
        }
    }

    private void ShowCurrentLine()
    {
        storyText.text = storyLines[currentLineIndex];
    }

    private void StartGameplay()
    {
        storyPanel.SetActive(false); // ストーリーパネルを閉じる

        if (dpTextObject != null)
        {
            dpTextObject.SetActive(true);
        }

        if (treasureMenu != null)
        {
            treasureMenu.SetActive(true);
        }
        
        Debug.Log("Game Started!");
    }
}
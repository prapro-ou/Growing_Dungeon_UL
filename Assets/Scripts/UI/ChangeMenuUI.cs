using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeMenuUI : MonoBehaviour
{

    
    [Header("Menu")]
    [SerializeField] private GameObject buildMenu;
    [SerializeField] private GameObject monsterMenu;
    [SerializeField] private GameObject treasureMenu;

    private void Start()
    {
        ShowBuildMenu();
    }

    public void ShowBuildMenu()
    {
        if (buildMenu != null)
        {
            buildMenu.SetActive(true);
            monsterMenu.SetActive(false);
            treasureMenu.SetActive(false);
        }
    }

    public void ShowMonsterMenu()
    {
        if (buildMenu != null)
            buildMenu.SetActive(false);

        if (monsterMenu != null)
            monsterMenu.SetActive(true);
    }

        public void ShowTreasureMenu()
    {
        if (buildMenu != null)
            buildMenu.SetActive(false);

        if (treasureMenu != null)
            treasureMenu.SetActive(true);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeMenuUI : MonoBehaviour
{

    
    [Header("Menu")]
    [SerializeField] private GameObject buildMenu;
    [SerializeField] private GameObject monsterMenu;

    private void Start()
    {
        ShowBuildMenu();
    }

    public void ShowBuildMenu()
    {
        if (buildMenu != null)
            buildMenu.SetActive(true);

        if (buildMenu != null)
            monsterMenu.SetActive(false);
    }

    public void ShowMonsterMenu()
    {
        if (buildMenu != null)
            buildMenu.SetActive(false);

        if (monsterMenu != null)
            monsterMenu.SetActive(true);
    }
}

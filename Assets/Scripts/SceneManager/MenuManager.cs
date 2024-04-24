using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject PressStart;
    public GameObject TittleMenu;
    public GameObject Menu;
    public GameObject NewGame;
    public GameObject Options;

    public void PressAnyKey()
    {
        PressStart.SetActive(false);
    
        TittleMenu.SetActive(true);
        Menu.SetActive(true);
    }

    public void PressNewGame()
    {
        Menu.SetActive(false);

        NewGame.SetActive(true);
    }

    public void PressOptions()
    {
        Menu.SetActive(false);

        Options.SetActive(true);
    }

    public void PressBack()
    {
        NewGame.SetActive(false);
        Options.SetActive(false);

        Menu.SetActive(true);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string Name;
    public void changeS()
    {
        SceneManager.LoadScene(Name);
    }

    public void Exit()
    {
        Application.Quit();
    }

}

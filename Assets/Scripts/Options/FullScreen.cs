using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FullScreen : MonoBehaviour
{
    public Toggle toggle;
    public TMP_Dropdown resolutionDropDown;
    Resolution[] resolucoes;
    void Start()
    {
        toggle.isOn = Screen.fullScreen ? true : false;

        UpdateResolucion();
    }

    public void ActiveFullScreen(bool active) { Screen.fullScreen = active; }

    public void UpdateResolucion()
    {
        resolucoes = Screen.resolutions;
        resolutionDropDown.ClearOptions();
        List<string> options = new List<string>();
        int resolucaoAtual = 0;

        for(int i = 0; i < resolucoes.Length; i++)
        {
            string option = $"{resolucoes[i].width} x {resolucoes[i].height} {resolucoes[i].refreshRate}Hz";
            options.Add(option);

            if(Screen.fullScreen && resolucoes[i].width == Screen.currentResolution.width &&
                resolucoes[i].height == Screen.currentResolution.height)
            {
                resolucaoAtual = i;
            }
        }
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = resolucaoAtual;
        resolutionDropDown.RefreshShownValue();

        resolutionDropDown.value = PlayerPrefs.GetInt("numeroResolution", 0);
    }

    public void ChangeResolution(int indiceResolution)
    {
        PlayerPrefs.SetInt("numeroResolution", resolutionDropDown.value);

        Resolution resolucao = resolucoes[indiceResolution];
        Screen.SetResolution(resolucao.width, resolucao.height, Screen.fullScreen);
    }

}

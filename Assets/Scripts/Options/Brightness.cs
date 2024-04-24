using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Brightness : MonoBehaviour
{
    public Slider brightnessSlider;
    private const string brightnessKey = "Brightness";

    void Start()
    {
        // Adicione um ouvinte ao controle deslizante
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChange);

        // Recupere o valor de brilho salvo, se existir
        if (PlayerPrefs.HasKey(brightnessKey))
        {
            float savedBrightness = PlayerPrefs.GetFloat(brightnessKey);
            brightnessSlider.value = savedBrightness;
            SetBrightness(savedBrightness);
        }
    }

    void OnBrightnessChange(float value)
    {
        SetBrightness(value);

        // Salve o valor de brilho atual
        PlayerPrefs.SetFloat(brightnessKey, value);
        PlayerPrefs.Save(); // Isso garante que as alterações sejam salvas imediatamente
    }

    void SetBrightness(float value)
    {
        // Certifique-se de que o valor está dentro da faixa permitida (0 a 1, por exemplo)
        value = Mathf.Clamp01(value);

        // Ajuste o brilho da tela conforme necessário
        // (substitua esta lógica pela necessária para o seu jogo)
        RenderSettings.ambientIntensity = value;
    }
}
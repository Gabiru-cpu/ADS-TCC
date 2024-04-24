using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    public Slider slider;
    public float sliderValue;
    public Image imagenMute;

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat("volumenAudio", .5f);
        sliderValue = slider.value; // Certifique-se de atualizar sliderValue com o valor do slider
        CheckMute();
        AudioListener.volume = sliderValue; // Mova isso para o final do método
    }

    public void ChangeSlider(float valor)
    {
        sliderValue = valor;
        PlayerPrefs.SetFloat("volumenAudio", sliderValue);
        CheckMute();
        AudioListener.volume = sliderValue; // Atualize o volume quando o slider for alterado
    }

    public void CheckMute()
    {
        imagenMute.enabled = (sliderValue == 0) ? true : false;
    }
}

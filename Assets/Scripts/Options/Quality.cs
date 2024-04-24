using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Quality : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public int quality;
    void Start()
    {
        quality = PlayerPrefs.GetInt("numeroDeQualidade", 3);
        dropdown.value= quality;
        UpdateQuality();
    }

    public void UpdateQuality()
    {
        QualitySettings.SetQualityLevel(dropdown.value);
        PlayerPrefs.SetInt("numeroDeQualidade", dropdown.value);
        quality = dropdown.value;
    }
}

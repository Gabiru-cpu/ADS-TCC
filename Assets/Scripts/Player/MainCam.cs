using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    public Xingu xingu;
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise; // Refer�ncia � propriedade de ru�do da c�mera

    private void Start()
    {        
       
        // Obtenha a propriedade de ru�do da c�mera
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;  // Destrava o cursor
            Cursor.visible = true;  // Torna o cursor vis�vel
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;  // Trava o cursor
            Cursor.visible = true;  // Torna o cursor invis�vel
        }

        if (xingu.archerMode)
        {
            noise.m_AmplitudeGain = 1;
            noise.m_FrequencyGain = 1;
        }
        else
        {
            noise.m_AmplitudeGain = 0;
            noise.m_FrequencyGain = 0;
        }
    }
}

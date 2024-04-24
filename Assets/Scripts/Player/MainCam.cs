using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCam : MonoBehaviour
{
    public Xingu xingu;
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise; // Referência à propriedade de ruído da câmera

    private void Start()
    {        
       
        // Obtenha a propriedade de ruído da câmera
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;  // Destrava o cursor
            Cursor.visible = true;  // Torna o cursor visível
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;  // Trava o cursor
            Cursor.visible = true;  // Torna o cursor invisível
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

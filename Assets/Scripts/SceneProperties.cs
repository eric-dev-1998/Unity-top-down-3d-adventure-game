using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneProperties : MonoBehaviour
{
    public bool enableDirectionalLight;
    public bool isDark;

    GameObject dirLight;

    public void Start()
    {
        dirLight = GameObject.Find("Sun");
    }

    public void applySettings()
    {
        dirLight.SetActive(enableDirectionalLight);

        if (isDark)
        {
            RenderSettings.ambientSkyColor = Color.black;
            RenderSettings.reflectionIntensity = 0f;
        }
        else
        {
            RenderSettings.ambientSkyColor = Color.grey;
            RenderSettings.reflectionIntensity = 1f;
        }
    }
}

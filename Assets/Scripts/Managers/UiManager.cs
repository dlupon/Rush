using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    // Components
    private CanvasScaler _scaler;

    private void Awake()
    {
        SetComponents();
        SetResolution();
    }

    private void SetComponents()
    {
        _scaler = GetComponent<CanvasScaler>();
    }

    private void SetResolution()
    {
        if (Application.platform == RuntimePlatform.Android) _scaler.scaleFactor = 2;
    }
}

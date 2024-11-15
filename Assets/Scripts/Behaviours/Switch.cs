using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Switch : MonoBehaviour
{
    public float Orientation { get => SwitchOrientation(); }
    [SerializeField] private float _orintantation = 1;

    private float SwitchOrientation()
    {
        print(_orintantation);
        _orintantation *= -1;
        return _orintantation;
    }
}
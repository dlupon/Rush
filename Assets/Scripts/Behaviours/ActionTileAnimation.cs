using Com.UnBocal.Rush.Properties;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActionTileAnimation : MonoBehaviour
{
    // Components
    [SerializeField] private Transform _transformAction;
    [SerializeField] private Transform _rootTransform;

    // Properties
    private Quaternion _baseRotation;

    // Animation
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _angle = 45f;
    private Vector3 _forward;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Start()
    {
        SetProperties();
    }

    private void Update()
    {
        UpdateRotation();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void SetProperties()
    {
        _forward = _rootTransform.forward;
        _baseRotation = _transformAction.rotation;
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Animation
    private void UpdateRotation()
    {
        float lRatio = Mathf.Cos(Game.Properties.TickRatio * Mathf.PI * 2);
        Quaternion lRotation = Quaternion.AngleAxis(_angle * lRatio, transform.forward);

        _transformAction.rotation = lRotation * _baseRotation;
    }
}

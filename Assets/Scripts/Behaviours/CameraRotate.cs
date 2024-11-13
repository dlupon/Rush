using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraRotate : MonoBehaviour
{
    // Rotation
    private Transform _transform;
    private Transform _targetTransform;

    // Inputs
    private Vector3 _inputLastPosition = default;
    private Vector3 _inputPosition = default;
    private Vector3 _inputDirection = default;
    private float _inputStrength = default;


    // Rotation Properties
    [SerializeField] private float _distance = 10f;
    [SerializeField] private Vector2 _speed = Vector2.one;

    private void Start()
    {
        _transform = transform;
        SetTarget();
    }

    private void Update()
    {
        UpdateInput();
        RotateOnInput();
    }

    private void SetTarget()
    {
        _targetTransform = new GameObject().transform;
        _targetTransform.position = _transform.position;
    }

    private void UpdateInput()
    {
        _inputPosition = Input.mousePosition;
        _inputDirection = (_inputPosition - _inputLastPosition).normalized;
        _inputStrength = (_inputPosition - _inputLastPosition).magnitude;
        _inputLastPosition = _inputPosition;
    }

    private void RotateOnInput()
    {
        if (!Input.GetMouseButton(1)) return;

        _transform.RotateAround(Vector3.zero , Vector3.up, _speed.x * _inputStrength * _inputDirection.x * Time.deltaTime);
        _transform.RotateAround(Vector3.zero , transform.right, _speed.y * -_inputStrength * _inputDirection.y * Time.deltaTime);
    }
}
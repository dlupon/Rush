using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMouseRotation : MonoBehaviour
{
    private Camera _camera;

    [SerializeField] private float _strength = 10f;
    [SerializeField] private float _speed = 10f;
    private Quaternion _basedRotation;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _basedRotation = transform.rotation;
    }

    private void Update()
    {
        Vector3 _cameraScale = (_camera.scaledPixelHeight * Vector3.up + _camera.scaledPixelWidth * Vector3.right);
        Vector3 _mousePosition = (Input.mousePosition - _cameraScale * .5f);
        _mousePosition = _mousePosition / (_cameraScale.magnitude * .5f) * _strength;

        Quaternion rotation = Quaternion.AngleAxis(_mousePosition.x, transform.up);
        rotation *= Quaternion.AngleAxis(-_mousePosition.y, transform.right);

        transform.rotation = Quaternion.Lerp(transform.rotation, _basedRotation * rotation, _speed * Time.deltaTime);
    }
}

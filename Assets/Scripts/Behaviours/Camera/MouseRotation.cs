using UnityEngine;
using UnityEngine.UI;

public class CameraMouseRotation : MonoBehaviour
{
    private Camera _camera;

    [SerializeField] private float _strength = 10f;
    [SerializeField] private float _speed = 10f;
    private Quaternion _basedRotation;

    private Vector3 _baseResolution;

    private void Awake()
    {
        _camera = Camera.main;
        _basedRotation = transform.rotation;
        _baseResolution = Screen.width * Vector3.right + Screen.height * Vector3.up;
    }

    private void Update()
    {
        Vector3 _mousePosition = (Input.mousePosition - _baseResolution * .5f);
        _mousePosition = _mousePosition / (_baseResolution.magnitude * .5f) * _strength;

        Quaternion rotation = Quaternion.AngleAxis(_mousePosition.x, transform.up);
        rotation *= Quaternion.AngleAxis(-_mousePosition.y, transform.right);

        transform.rotation = Quaternion.Lerp(transform.rotation, _basedRotation * rotation, _speed * Time.deltaTime);
    }
}

using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    // Rotation
    private Transform _transform;
    private Transform _targetTransform;

    // Inputs
    private Vector3 _inputLastPosition = default;
    private Vector3 _inputPosition = default;
    private Vector3 _inputDirection = default;
    private float _inputScroll = default;
    private float _inputStrength = default;


    // Rotation Properties
    [SerializeField] private Vector2 _speed = new Vector3(30f, 15f);

    // Position Properties
    [SerializeField] private float _distanceMin = 13f;
    [SerializeField] private float _distanceMax = 30f;
    [SerializeField] private float _distanceSpeed = 10f;
    private float _distance = 0f;

    private void Start()
    {
        _transform = transform;
        _transform.position = -Vector3.forward;
        _distance = _distanceMin;
    }

    private void Update()
    {
        UpdateInput();
        RotateOnInput();
        DistanceOnInput();
    }

    private void UpdateInput()
    {
        _inputPosition = Input.mousePosition;
        _inputScroll = Input.mouseScrollDelta.y;

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

    private void DistanceOnInput()
    {
        _distance = Mathf.Clamp(_distance - _inputScroll, _distanceMin, _distanceMax);
        Vector3 l_toCamera = _transform.position.normalized;
        _transform.position = Vector3.Lerp(_transform.position, l_toCamera * _distance, _distanceSpeed * Time.deltaTime);
    }
}
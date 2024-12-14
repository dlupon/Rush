using UnityEngine;
using Com.UnBocal.Rush.Properties;
using Unity.VisualScripting;
using UnityEngine.ProBuilder;
using Unity.VisualScripting.FullSerializer;

public class RotateAround : MonoBehaviour
{
    // Components
    private Transform _transform;

    // Target
    [SerializeField] private Transform _target;
    private Vector3 _totarget;

    // Rotation
    [SerializeField] private float _rotationSpeed = 30f;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Awake()
    {
        SetComponents();
    }

    private void Start()
    {
        SetTargetProperties();
    }

    private void Update()
    {
        RotateAroundTarget();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void SetComponents()
    {
        _transform = GetComponent<Transform>();
    }

    private void SetTargetProperties()
    {
        _totarget = _transform.position - _target.position;
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Rotate
    private void RotateAroundTarget()
    {
        _totarget = Quaternion.AngleAxis(_rotationSpeed * Time.deltaTime, Vector3.up) * _totarget;
        _transform.position = _target.position + _totarget;
        _transform.LookAt(_target.position);
    }
}
using DG.Tweening;
using UnityEngine;
using Com.UnBocal.Rush.Managers;
using Com.UnBocal.Rush.Properties;
using UnityEditor;

public class TilePlacer : MonoBehaviour
{
    private Transform _transform;
    private Camera _camera;

    private void Start()
    {
        _transform = transform;
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0)) return;

        Vector3 _pos = Game.Inputs.MousePosition;
        Ray _raycast = _camera.ScreenPointToRay(_pos);

        RaycastHit _hit;
        bool _hitFound = Physics.Raycast(_raycast, out _hit);
        if (!_hitFound) return;
        // if (_hit.collider.tag != "Tile") return;

        Transform _colliderTransform = _hit.collider.transform;
        if (!_colliderTransform.TryGetComponent(out JuicyTouch l_JT)) return;

        l_JT.Shake();


        Debug.DrawLine(_transform.position - _transform.up * .1f, _hit.point, Color.blue);
    }
}
using DG.Tweening;
using UnityEngine;
using Com.UnBocal.Rush.Managers;

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

        Ray _raycast = _camera.ScreenPointToRay(Input.mousePosition);

        RaycastHit _hit;
        bool _hitFound = Physics.Raycast(_raycast, out _hit);
        if (!_hitFound) return;
        if (_hit.collider.tag != "Tile") return;

        Transform _colliderTransform = _hit.collider.transform;
        _colliderTransform.DOKill();
        _colliderTransform.rotation = Quaternion.identity;
        _colliderTransform.DOShakeRotation(1f, 10f);

        Debug.DrawLine(_transform.position - _transform.up * .1f, _colliderTransform.position, Color.blue);
    }
}

using UnityEngine;
using Com.UnBocal.Rush.Properties;
using Unity.VisualScripting;
using UnityEngine.ProBuilder;
using Com.UnBocal.Rush.Managers;
using System.Collections.Generic;

public class TilePlacer : MonoBehaviour
{
    // Components
    private Transform _transform;
    private Camera _camera;

    // Tile
    [SerializeField] private float _tileFollowingSpeed = 10f;
    [SerializeField] private LayerMask _layerPlacing;
    [SerializeField] private LayerMask _LayerIngame;
    private Game.Properties.ActionTile _actionTile = default;
    private Transform _currentTile = null;
    private Vector3 _offsetDefault = Vector3.up;
    private Vector3 _offsetOnPlacing = Vector3.up * .6f;
    private List<GameObject> _tiles = new List<GameObject>();

    // Raycast
    private enum ColliderType { None, Tile, Other }
    private const float RAYCAST_MAX_DISTANCE = 100f;
    private Ray _raycast;
    private RaycastHit _hit;
    private Transform _HitColliderTrasnform;
    private bool _hitFound;

    // Juice
    [SerializeField] private int _shakeForce = 3;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Awake()
    {
        SetComponents();
        ConnectEvents();
    }

    private void Update()
    {
        UpdateRaycast();
        UpdateCurrentTile();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void SetComponents()
    {
        _transform = transform;
        _camera = GetComponent<Camera>();
    }

    private void ConnectEvents()
    {
        Game.Events.TileSelected.AddListener(SetTile);
        Game.Events.End.AddListener(Reset);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tile
    private void Reset()
    {
        foreach (GameObject lTile in _tiles) Destroy(lTile);
        _tiles.Clear();
    }

    private void SetTile(Game.Properties.ActionTile pActionTile)
    {
        _actionTile = pActionTile;
        if (!_actionTile.CanPlaceNewTile) return;
        if (_currentTile != null) Destroy(_currentTile.gameObject);
        SetNewTile();
    }

    private void SetNewTile()
    {
        Quaternion lRotation = Quaternion.AngleAxis(Game.Properties.ROTATION * (float)_actionTile.Direction, Vector3.up);
        _currentTile = Instantiate(_actionTile.Factory, Vector3.zero, lRotation).transform;
        _currentTile.gameObject.layer = Game.Properties.LayerIgnore;
    }

    private void UpdateCurrentTile()
    {
        if (_currentTile == null) return;
        UpdateCurrentTileVisibility();

        if (!_hitFound) return;
        if (Game.Properties.Running) { Juice(); return; }
        UpdateCurrentTilePosition();

        if (!Input.GetMouseButtonDown(0)) return;
        if (CheckRemoveTile()) return;
        CheckPlacingTile();
    }

    private void UpdateCurrentTileVisibility()
    {
        if (Game.Properties.Running) { _currentTile.gameObject.SetActive(false); return; }

        bool lIsVisible = _hitFound && _actionTile.CanPlaceNewTile;
        if (_currentTile.gameObject.activeSelf == lIsVisible) return;
        _currentTile.gameObject.SetActive(lIsVisible);
    }

    private void UpdateCurrentTilePosition()
    {
        _currentTile.position = _HitColliderTrasnform.position + _offsetDefault;
    }

    private void Juice()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Game.Events.LevelTouched.Invoke(_HitColliderTrasnform.position);
        
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Placing / Removing
    private void CheckPlacingTile()
    {
        if (!(_actionTile.CanPlaceNewTile && CheckAboveTile() == ColliderType.None)) return;
        PlacingTile();
        SetNewTile();
    }

    private bool CheckRemoveTile()
    {
        if (_hit.collider.gameObject.layer != Game.Properties.LayerActionTile) return false;
        if (!IsActionTile()) return true;

        RemoveTile();
        return true;
    }

    private void PlacingTile()
    {
        _currentTile.transform.parent = _HitColliderTrasnform.TryGetComponent(out JuicyCapter lCapter) ? lCapter.CapterTransform : _HitColliderTrasnform;

        _currentTile.position = _HitColliderTrasnform.position + _offsetOnPlacing;
        _currentTile.gameObject.layer = Game.Properties.LayerActionTile;
        _actionTile.AddTile(_currentTile);

        _tiles.Add(_currentTile.gameObject);

        Game.Events.TilePlaced.Invoke(_currentTile);
        Game.Events.TileUpdate.Invoke(_actionTile);
    }

    private void RemoveTile()
    {
        Destroy(_hit.collider.gameObject);

        _tiles.Remove(_hit.collider.gameObject);

        Game.Events.TileUpdateRemove.Invoke(_hit.collider.gameObject);
    }

    private bool IsActionTile()
    {
        switch (_hit.collider.gameObject.tag)
        {
            case "Stopper" : return true;
            case "Conveyor" : return true;
            case "Switch" : return true;
            case "Arrow" : return true;
            default: return false;
        }
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // RayCast
    private void UpdateRaycast()
    {
        _raycast = _camera.ScreenPointToRay(Game.Inputs.MousePosition);
        _hitFound = Physics.Raycast(_raycast, out _hit, RAYCAST_MAX_DISTANCE, Game.Properties.Running ? _LayerIngame : _layerPlacing);

        if (!_hitFound) return;

        _HitColliderTrasnform = _hit.collider.transform;
    }

    private ColliderType CheckAboveTile()
    {
        Vector3 lPosition = _HitColliderTrasnform.position;
        bool lHitFound = Physics.Raycast(lPosition, Vector3.up, out RaycastHit lHit, 1f);
        if (!lHitFound) return ColliderType.None;
        if (lHit.collider.gameObject.layer == Game.Properties.LayerIgnore) return ColliderType.None;
        if (lHit.collider.gameObject.layer == Game.Properties.LayerTile) return ColliderType.Tile;
        return ColliderType.Other;
    }
}
using UnityEngine;
using Com.UnBocal.Rush.Properties;
using Unity.VisualScripting;
using UnityEngine.ProBuilder;

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

    // Raycast
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
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tile
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

        if (_currentTile.gameObject.activeSelf == _hitFound) return;
        _currentTile.gameObject.SetActive(_hitFound);
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
        if (!_actionTile.CanPlaceNewTile) return;
        PlacingTile();
        SetNewTile();
    }

    private bool CheckRemoveTile()
    {
        if (_hit.collider.gameObject.layer != Game.Properties.LayerActionTile) return false;
        RemoveTile();
        return true;
    }

    private void PlacingTile()
    {
        _currentTile.position = _HitColliderTrasnform.position + _offsetOnPlacing;
        _currentTile.gameObject.layer = Game.Properties.LayerActionTile;
        _actionTile.AddTile(_currentTile);

        Game.Events.TilePlaced.Invoke(_currentTile);
    }

    private void RemoveTile()
    {
        _actionTile.RemoveTile(_hit.collider.gameObject);
        Destroy(_hit.collider.gameObject);
    }


    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // RayCast
    private void UpdateRaycast()
    {
        _raycast = _camera.ScreenPointToRay(Game.Inputs.MousePosition);
        _hitFound = Physics.Raycast(_raycast, out _hit, RAYCAST_MAX_DISTANCE, Game.Properties.Running ? _LayerIngame : _layerPlacing);

        if (!_hitFound) return;
        // if (_hit.collider.gameObject.tag != "Tile" && !Game.Properties.Running) { _hitFound = false; return; }

        _HitColliderTrasnform = _hit.collider.transform;
    }
}
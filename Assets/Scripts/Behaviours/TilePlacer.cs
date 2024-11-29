using UnityEngine;
using Com.UnBocal.Rush.Properties;
using Unity.VisualScripting;

public class TilePlacer : MonoBehaviour
{
    // Components
    private Transform _transform;
    private Camera _camera;

    // Tile
    [SerializeField] private float _tileFollowingSpeed = 10f;
    [SerializeField] private LayerMask _layerTile;
    [SerializeField] private LayerMask _LayerJuicy;
    private Game.Properties.ActionTile _actionTile = default;
    private Transform _currentTile = null;
    private Vector3 _offsetDefault = Vector3.up;
    private Vector3 _offsetOnPlacing = Vector3.up * .5f;

    // Raycast
    private const float RAYCAST_MAX_DISTANCE = 100f;
    private Ray _raycast;
    private RaycastHit _hit;
    private Transform _HitColliderTrasnform;
    private bool _hitFound;

    // Check
    private bool _canUseTile => _hitFound && _currentTile != null;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Awake()
    {
        SetComponents();
        ConnectEvents();
    }

    private void Start()
    {
        SetFirstTile();
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
    private void SetFirstTile()
    {
        if (Game.Properties.CurrentActionTiles.Length <= 0) return;
        SetTile(Game.Properties.CurrentActionTiles[0]);
    }

    private void SetTile(Game.Properties.ActionTile pActionTile)
    {
        _actionTile = pActionTile;
        if (_currentTile != null) Destroy(_currentTile.gameObject);
        SetNewTile();
    }

    private void SetNewTile()
    {
        _currentTile = Instantiate(_actionTile.Factory).transform;
        _currentTile.rotation = Quaternion.AngleAxis(Game.Properties.ROTATION * (float)_actionTile.Direction, Vector3.up);
        _currentTile.gameObject.layer = Game.Properties.LayerIgnore;
    }

    private void UpdateCurrentTile()
    {
        if (!_canUseTile) return;
        if (Game.Properties.Running) { Juice();  return; }

        // CheckRemoveTile();
        UpdateCurrentTilePosition();
        CheckPlacingTile();
    }

    private void UpdateCurrentTilePosition()
    {
        _currentTile.position = _HitColliderTrasnform.position + _offsetDefault;
    }

    private void Juice()
    {
        if (!Input.GetMouseButton(0)) return;
        if (!_hit.collider.TryGetComponent(out JuicyTouch lJT)) return;
        lJT.Shake();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Placing
    private void CheckPlacingTile()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        PlacingTile();
        SetNewTile();
    }

    private void CheckRemoveTile()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        RemoveTile();
    }

    private void PlacingTile()
    {
        _currentTile.position = _HitColliderTrasnform.position + _offsetOnPlacing;
        _currentTile.gameObject.layer = Game.Properties.LayerTile;
    }

    private void RemoveTile()
    {
        bool test = Physics.Raycast(_raycast, out _hit, RAYCAST_MAX_DISTANCE);
        if (!test) return;
        // if (_hit.collider.gameObject.tag == null) {
    }


    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // RayCast

    private void UpdateRaycast()
    {
        _raycast = _camera.ScreenPointToRay(Game.Inputs.MousePosition);
        _hitFound = Physics.Raycast(_raycast, out _hit, RAYCAST_MAX_DISTANCE, Game.Properties.Running ? _LayerJuicy : _layerTile);
        if (!_hitFound) return;
        if (_hit.collider.gameObject.tag != "Tile" && !Game.Properties.Running) { _hitFound = false; return; }
        _HitColliderTrasnform = _hit.collider.transform;
    }
}
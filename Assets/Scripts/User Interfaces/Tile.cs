using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using static Com.UnBocal.Rush.Properties.Game.Properties;
using TMPro;
using Com.UnBocal.Rush.Properties;
using DG.Tweening;
using Unity.VisualScripting;

public class Tile : MonoBehaviour
{
    // Events
    [HideInInspector] public UnityEvent<ActionTile> Click = new UnityEvent<ActionTile>();

    // Components
    [SerializeField] private Transform _tileParent;
    [SerializeField] private Text _count;
    private Button _button;
    private RectTransform _rectTransform;
    private RawImage _rawImage;

    // Tile
    [SerializeField] private LayerMask _layer;
    private Transform _tile;
    private ActionTile _actionTile;
    private Quaternion _baseRotation;

    // Render
    [SerializeField] private Vector2Int _size = Vector2Int.one * 100;
    private RenderTexture _renderTexture;

    // Camera
    [SerializeField] private Camera _camera;
    [SerializeField] private float _cameraDistance = 5f;
    private Transform _mainCamera;

    private void Awake()
    {
        
    }

    private void Start()
    {
        SetComponents();
        ConnectEvents();
        SetRenderTexture();
    }

    private void Update() => UpdateCameraPosition();

    private void SetComponents()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rawImage = GetComponent<RawImage>();
        _button = GetComponent<Button>();
        _mainCamera = Camera.main.transform;
    }

    private void ConnectEvents()
    {
        _button.onClick.AddListener(OnClick);
        Game.Events.TileUpdate.AddListener(OnUpdateActionTile);
    }

    private void SetTileProperties()
    {
        UpdateCount();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Button
    public void ForceClick() => OnClick();

    private void OnClick()
    {
        Click.Invoke(_actionTile);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // ActionTile
    private void OnUpdateActionTile(ActionTile pActionTile)
    {
        if (pActionTile != _actionTile) return;
        UpdateCount();
    }

    private void UpdateCount()
    {
        _count.text = $"{_actionTile.Left}";
        bool lCountAboveZero = _actionTile.Left > 0;
        if (gameObject.activeSelf == lCountAboveZero) return;
        gameObject.SetActive(lCountAboveZero);
        if (!lCountAboveZero) Game.Events.TileUpdateCount.Invoke();
    }


    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Rendere
    #region Renderer
    private void UpdateCameraPosition()
    {
        Vector3 lToCamera = Vector3.ProjectOnPlane(_mainCamera.forward, Vector3.down).normalized;
        _tile.rotation = _baseRotation * Quaternion.Inverse(Quaternion.LookRotation(lToCamera));
    }

    public void SetTile(ActionTile pActionTile)
    {
        _tile = Instantiate(pActionTile.Factory, _tileParent).transform;

        _tile.position = _tileParent.position;
        _tile.rotation = Quaternion.AngleAxis(ROTATION * (float)pActionTile.Direction, Vector3.up);

        _baseRotation = _tile.rotation;

        _actionTile = pActionTile;

        SetLayer(_tile);
        SetTileProperties();
    }

    private void SetRenderTexture()
    {
        _renderTexture = new RenderTexture(_size.x, _size.y, 16);
        _renderTexture.filterMode = FilterMode.Point;
        _camera.targetTexture = _renderTexture;
        _rawImage.texture = _renderTexture;
    }

    private void SetLayer(Transform pTarget)
    {
        pTarget.gameObject.layer = gameObject.layer;
        if (pTarget.childCount == 0) return;
        int lChildCout = pTarget.childCount;

        for (int lChildIndex = 0; lChildIndex < lChildCout; lChildIndex++)
            SetLayer(pTarget.GetChild(lChildIndex).transform);
    }
    #endregion
}
using UnityEngine;
using Com.UnBocal.Rush.Properties;
using UnityEngine.UI;

public class TileRenderer : MonoBehaviour
{
    // Components

    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _tileParent;
    private RectTransform _rectTransform;
    private RawImage _rawImage;

    // Tile
    [SerializeField] private LayerMask _layer;
    private Transform _tile;

    // Render
    [SerializeField] private Vector2Int _size = Vector2Int.one * 100;
    private RenderTexture _renderTexture;

    private void Awake() => SetComponents();

    private void SetComponents()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rawImage = GetComponent<RawImage>();
    }

    public void SetTile(GameObject pTileFactory, Game.Properties.Orientation pDirection)
    {
        _tile = Instantiate(pTileFactory, _tileParent).transform;
        _tile.position = _tileParent.position;
        _tile.rotation = Quaternion.AngleAxis(Game.Properties.ROTATION * (float)pDirection, Vector3.up);
        SetRenderTexture();
        SetLayer(_tile);
    }

    private void SetRenderTexture()
    {
        _renderTexture = new RenderTexture(_size.x, _size.y, 16);
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
}

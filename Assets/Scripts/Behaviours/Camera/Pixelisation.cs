using Com.UnBocal.Rush.Properties;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Pixelisation : MonoBehaviour
{
    [SerializeField] private RawImage _image = null;
    private Camera _camera;

    [SerializeField, Range(0.0025f,1f)] private float _ratio = .3f;

    private Vector2Int _currentResolution = Vector2Int.zero;

    private void Awake()
    {
        SetComponents();
        UpdateResolution();
    }

    private void FixedUpdate()
    {
        UpdateResolution();
    }

    private void SetComponents() => _camera = Camera.main;

    private void UpdateResolution()
    {
        if (_image == null) return;

        Game.Properties.SetScreenPixelization(_ratio);

        if (_camera.targetTexture != null)
        {
            if (Game.Properties.ScreenSizeWithPixelization.x == _camera.targetTexture.width && Game.Properties.ScreenSizeWithPixelization.y == _camera.targetTexture.height) return;
            Destroy(_camera.targetTexture);
        }

        _camera.targetTexture = new RenderTexture(Game.Properties.ScreenSizeWithPixelization.x, Game.Properties.ScreenSizeWithPixelization.y, 16);

        _camera.targetTexture.antiAliasing = 1;
        _camera.targetTexture.filterMode = FilterMode.Point;

        _image.texture = _camera.targetTexture;
    }
}

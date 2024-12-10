using UnityEngine;
using Com.UnBocal.Rush.Properties;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class HUD : Interface
{
    // Buttons
    [SerializeField] private Slider _slider;
    [SerializeField] private Button _playButton;

    // Action Tiles
    [SerializeField] private Transform _tileParent;
    [SerializeField] private GameObject _tileFactory;
    private Game.Properties.ActionTile[] _actionTiles;

    // Animations
    private const float ANIMATION_END_DURATION = 1f;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Start()
    {
        SelectFirstTile();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    protected override void ConnectEvents()
    {
        Game.Events.ActionTilesUpdated.AddListener(SetTiles);
        _slider.onValueChanged.AddListener(OnSliderChange);
        _playButton.onClick.AddListener(OnPlayClick);
        Game.Events.End.AddListener(OnEnd);
    }

    private void SetTiles(Game.Properties.ActionTile[] pActionTiles)
    {
        _actionTiles = pActionTiles;
        CreateHud();
    }

    private void CreateHud()
    {
        foreach (Game.Properties.ActionTile lCurrentActionTile in _actionTiles)
            CreateTile(lCurrentActionTile);
    }

    private void CreateTile(Game.Properties.ActionTile pCurrentActionTile)
    {
        Transform lCurrentTile = Instantiate(_tileFactory, _tileParent).transform;
        Tile lTile = lCurrentTile.GetComponent<Tile>();
        lCurrentTile.GetComponent<Tile>().SetTile(pCurrentActionTile);
        lTile.Click.AddListener(OnTileClick);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Game
    private void OnEnd()
    {
        Disable();
        PlayEndAnimation();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // UI
    private void SelectFirstTile()
    {
        if (_actionTiles.Length <= 0) return;
        OnTileClick(_actionTiles[0]);
    }
    
    private void OnTileClick(Game.Properties.ActionTile pTileFactory)
    {
        if (!m_inputReactive) return;
        Game.Events.TileSelected.Invoke(pTileFactory);
    }

    private void OnPlayClick()
    {
        if (!m_inputReactive) return;
        Game.Events.ToggleRuning.Invoke();
    }

    // Slider
    private void OnSliderChange(float pValue)
    {
        if (!m_inputReactive) return;
        Game.Properties.SliderValue = pValue;
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Animation
    private void PlayEndAnimation()
    {
        m_rectTransform.DOScale(Vector3.one * 2f, ANIMATION_END_DURATION).SetEase(Ease.InExpo).onComplete = Delete;
    }
}
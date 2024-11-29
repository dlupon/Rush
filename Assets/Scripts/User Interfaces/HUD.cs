using UnityEngine;
using Com.UnBocal.Rush.Properties;

public class HUD : MonoBehaviour
{
    // Action Tiles
    [SerializeField] private Transform _tileParent;
    [SerializeField] private GameObject _tileFactory;
    private Game.Properties.ActionTile[] _actionTiles;

    private void Awake()
    {
        ConnectEvents();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void ConnectEvents()
    {
        Game.Events.ActionTilesUpdated.AddListener(SetTiles);
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
        Transform lCurrentTile = Instantiate(_tileFactory).transform;
        Tile lTile = lCurrentTile.GetComponent<Tile>();
        lCurrentTile.GetComponent<Tile>().SetTile(pCurrentActionTile);
        lCurrentTile.SetParent(_tileParent);

        lTile.Click.AddListener(OnTileClick);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // UI
    private void OnTileClick(Game.Properties.ActionTile pTileFactory)
    {
        Game.Events.TileSelected.Invoke(pTileFactory);
    }
}
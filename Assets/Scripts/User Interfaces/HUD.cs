using UnityEngine;
using Com.UnBocal.Rush.Properties;

public class HUD : MonoBehaviour
{
    // Action Tiles
    [SerializeField] private Transform _tileParent;
    [SerializeField] private GameObject _tileFactory;
    private Game.Properties.ActionTile[] _actionTiles;

    private void Start()
    {
        ConnectEvents();
    }

    private void ConnectEvents()
    {
        Game.Events.ActionTilesSeted.AddListener(SetTiles);
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
        lCurrentTile.GetComponent<TileRenderer>().SetTile(pCurrentActionTile.Factory, pCurrentActionTile.Direction);
        lCurrentTile.SetParent(_tileParent);
    }
}
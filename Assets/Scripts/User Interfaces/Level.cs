using UnityEngine;
using Com.UnBocal.Rush.Properties;

public class Level : MonoBehaviour
{
    [SerializeField] private Game.Properties.ActionTile[] _actionTiles;

    private void Start()
    {
        SetActionTiles();
    }

    private void SetActionTiles()
    {
        Game.Properties.SetCurrentActionTiles(_actionTiles);
    }
}
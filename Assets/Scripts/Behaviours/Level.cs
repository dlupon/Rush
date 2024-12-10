using UnityEngine;
using Com.UnBocal.Rush.Properties;

public class Level : MonoBehaviour
{
    [SerializeField] private Game.Properties.ActionTile[] _actionTiles;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Start()
    {
        SetActionTiles();
        SetLevel();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void SetActionTiles()
    {
        Game.Properties.SetCurrentActionTiles(_actionTiles);
    }

    private void SetLevel()
    {
        Game.Properties.SetLevel(transform);
    }
}
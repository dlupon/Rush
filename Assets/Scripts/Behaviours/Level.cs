using UnityEngine;
using Com.UnBocal.Rush.Properties;
using TMPro;

public class Level : MonoBehaviour
{
    [SerializeField] private Game.Properties.ActionTile[] _actionTiles;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Load
    public void Launch()
    {
        SetActionTiles();
        SetLevel();
        Game.Properties.InGame = true;
    }

    private void SetActionTiles()
    {
        Game.Properties.SetCurrentActionTiles(_actionTiles);
    }

    private void SetLevel()
    {
        transform.position = Vector3.zero;
        Game.Properties.SetLevel(transform);
    }
}
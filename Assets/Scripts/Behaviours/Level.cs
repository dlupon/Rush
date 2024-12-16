using UnityEngine;
using Com.UnBocal.Rush.Properties;
using TMPro;

public class Level : MonoBehaviour
{
    [SerializeField] private Game.Properties.ActionTile[] _actionTiles;

    [SerializeField] private Vector3 _center = Vector3.zero;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Load
    public void Launch()
    {
        SetActionTiles();
        SetLevel();
    }

    private void SetActionTiles()
    {
        Game.Properties.SetCurrentActionTiles(_actionTiles);
    }

    private void SetLevel()
    {
        transform.position = Vector3.zero;
        Game.Properties.SetLevel(transform, _center);
    }
}
using UnityEngine;
using Com.UnBocal.Rush.Properties;

public class Typable : MonoBehaviour
{
    public Game.Properties.CubeType CubeType => _cubeType;
    [SerializeField] private Game.Properties.CubeType _cubeType = Game.Properties.CubeType.WHITE;

    public void SetCubeType(Game.Properties.CubeType pCubeType) => _cubeType = pCubeType;
}
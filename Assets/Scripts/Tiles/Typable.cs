using UnityEngine;
using Com.UnBocal.Rush.Properties;

public class Typable : MonoBehaviour
{
    // Type                                               
    public Game.Properties.CubeType CubeType => _cubeType;
    [SerializeField] private Game.Properties.CubeType _cubeType = Game.Properties.CubeType.WHITE;

    // Renderer
    [SerializeField] private Renderer _renderer;

    public void SetCubeType(Game.Properties.CubeType pCubeType) => _cubeType = pCubeType;
}
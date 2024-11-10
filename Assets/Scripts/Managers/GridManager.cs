using Com.UnBocal.Rush.Properties;
using UnityEngine;

namespace Com.UnBocal.Rush.Managers
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        private void Awake() => Game.Properties.SetGrid(_grid);
    }
}
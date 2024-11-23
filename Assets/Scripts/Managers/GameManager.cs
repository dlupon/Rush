using UnityEngine;
using Com.UnBocal.Rush.Properties;
using UnityEngine.AI;

namespace Com.UnBocal.Rush.Managers
{
    public class GameManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
        private void Awake()
        {
            SetComponents();
        }

        private void Update()
        {
            UpdateInput();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Inputs
        private void UpdateInput()
        {
            if (!Game.Inputs.Run) return;
            Game.Properties.ToggleRunning();
            print($"RUNNING => {Game.Properties.Running}");
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Setters
        private void SetComponents()
        {

        }
    }
}
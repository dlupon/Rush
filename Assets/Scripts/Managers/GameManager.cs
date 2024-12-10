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
            ConnectEvents();
        }

        private void Update()
        {
            UpdateInput();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
        private void SetComponents()
        {

        }
        private void ConnectEvents()
        {
            Game.Events.ToggleRuning.AddListener(ToggleRunGame);
            Game.Events.End.AddListener(OnEnd);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Inputs
        private void UpdateInput()
        {
            if (Game.Inputs.Run) ToggleRunGame();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Game
        private void ToggleRunGame()
        {
            Game.Properties.ToggleRunning();
            Time.timeScale = 1f;
        }

        private void OnEnd()
        {
            Game.Properties.SetRunning(false);
        }
    }
}
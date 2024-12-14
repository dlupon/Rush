using UnityEngine;
using Com.UnBocal.Rush.Properties;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class MainMenu : Interface
{
    private bool _inputReactive = true;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Update()
    {
        UpdateInput();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    protected override void ConnectEvents()
    {
        
    }

    protected override void DisconnectEvents()
    {
    
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Input
    private void UpdateInput()
    {
        if (!_inputReactive) return;
        if (Input.GetMouseButtonDown(0)) LaunchGame();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // UI
    private void LaunchGame()
    {
        _inputReactive = false;
        Game.Events.LaunchGame.Invoke();
    }
}
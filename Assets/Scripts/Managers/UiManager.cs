using Com.UnBocal.Rush.Properties;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    // Components
    private CanvasScaler _scaler;
    private RectTransform _transform;

    // Game View
    [SerializeField] private GameObject _gameView;

    // UI
    [SerializeField] private GameObject _MainMenu;
    [SerializeField] private GameObject _LevelSelector;
    [SerializeField] private GameObject _HUD;
    [SerializeField] private GameObject _ErrorCollision;
    [SerializeField] private GameObject _ErrorFalling;

    // World
    [SerializeField] private Transform _worldParent;
    [SerializeField] private GameObject _worldMainMenu;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Awake()
    {
        SetComponents();
        ConnectEvents();
    }

    private void Start()
    {
        ResetUI();
        GoToMainMenue();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void SetComponents()
    {
        _scaler = GetComponent<CanvasScaler>();
        _transform = GetComponent<RectTransform>();
    }

    private void ConnectEvents()
    {
        Game.Events.LaunchGame.AddListener(OnLaunchGame);
        Game.Events.LaunchLevel.AddListener(OnPlay);
        Game.Events.Running.AddListener(HideErrors);
        Game.Events.StopRunning.AddListener(HideErrors);
        Game.Events.CubeDiedFromFalling.AddListener(ShowErrorFalling);
        Game.Events.CubeDiedFromCollision.AddListener(ShowErrorCollision);

        Game.Events.End.AddListener(OnLaunchGame);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Events
    private void OnLaunchGame()
    {
        ResetUI();
        GoToLevelSelector();
    }
    private void OnPlay()
    {
        ResetUI();
        GoToHUD();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Game View
    private void SetGameVisibility(bool pVisible) => _gameView.SetActive(pVisible);


    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // UI
    private void ResetUI()
    {
        int lInterfaceCount = _transform.childCount;
        int lWorldCount = _worldParent.childCount;
        GameObject lCurrent;

        for (int lCurrentInterfaceIndex = 0; lCurrentInterfaceIndex < lInterfaceCount; lCurrentInterfaceIndex++)
        {
            lCurrent = _transform.GetChild(lCurrentInterfaceIndex).gameObject;
            if (lCurrent == _gameView) continue;
            lCurrent.SetActive(false);
        }
        
        for (int lCurrentWorldIndex = 0; lCurrentWorldIndex < lWorldCount; lCurrentWorldIndex++)
        {
            lCurrent = _worldParent.GetChild(lCurrentWorldIndex).gameObject;
            lCurrent.SetActive(false);
        }
    }

    private void ResetWorld()
    {
        int lInterfaceCount = _worldParent.childCount;
        GameObject lCurrentInterface;

        for (int lCurrentInterfaceIndex = 0; lCurrentInterfaceIndex < lInterfaceCount; lCurrentInterfaceIndex++)
        {
            lCurrentInterface = _worldParent.GetChild(lCurrentInterfaceIndex).gameObject;
            lCurrentInterface.SetActive(false);
        }
    }

    private void GoToMainMenue()
    {
        ResetUI();
        SetGameVisibility(true);
        _MainMenu.SetActive(true);
        _worldMainMenu.SetActive(true);
    }

    private void GoToLevelSelector()
    {
        SetGameVisibility(false);
        _LevelSelector.SetActive(true);
    }

    private void GoToHUD()
    {
        SetGameVisibility(true);
        _HUD.SetActive(true);
    }

    private void ShowErrorCollision() => _ErrorCollision.SetActive(true);
    private void ShowErrorFalling() => _ErrorFalling.SetActive(true);
    private void HideErrors()
    {
        _ErrorFalling.SetActive(false);
        _ErrorCollision.SetActive(false);
    }
}

using UnityEngine;
using Com.UnBocal.Rush.Properties;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelSelector : Interface
{
    // Levels Buttons
    [SerializeField] private GameObject _levelButtonFactoty;
    [SerializeField] private Transform _buttonContainer;
    private List<LevelButton> _buttons = new List<LevelButton>();

    // Play
    [SerializeField] private Button _playButton;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Start()
    {
        OpenAnimation();
        SetLevel();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    protected override void ConnectEvents()
    {
        _playButton.onClick.AddListener(OnPlay);
    }

    private void CreateFolders()
    {
    
    }

    private void SetLevel()
    {
        if (Game.Properties.Levels == null) return;

        foreach (GameObject lLevel in Game.Properties.Levels) CreateNewLevelButton(lLevel);

        _buttons[0].Select();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Level Button
    private void CreateNewLevelButton(GameObject pLevel)
    {
        LevelButton lLevelButton = Instantiate(_levelButtonFactoty, _buttonContainer).GetComponent<LevelButton>();
        
        // lLevelButton.Click.AddListener(OnLevelClick);
        _buttons.Add(lLevelButton);

        lLevelButton.SetLevel(pLevel);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // UI
    private void OnLevelClick(int pLevelIndex)
    {
        if (!m_inputReactive) return;
    }

    private void OnPlay()
    {
        Game.Events.LaunchLevel.Invoke();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Animation
    private void OpenAnimation()
    {
        m_rectTransform.DOScale(Vector3.one, .5f).From(new Vector3(.9f, 1.1f, .9f)).SetEase(Ease.OutElastic);
    }
}
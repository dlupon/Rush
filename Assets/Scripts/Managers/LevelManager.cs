using Com.UnBocal.Rush.Properties;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering;

public class LevelManager : MonoBehaviour
{
    // Components
    private Transform _transform;

    // Levels
    private int _levelCount => _transform.childCount;
    private List<GameObject> _levels = new List<GameObject>();
    private Level _currentLevel;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Awake()
    {
        SetComponents();
        ConnectEvents();
        SetLevels();
    }

    private void Start()
    {
        UnloadAllLevels();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void SetComponents()
    {
        _transform = GetComponent<Transform>();
    }
    
    private void ConnectEvents()
    {
        Game.Events.LevelLoad.AddListener(OnLevelLoad);
        Game.Events.LaunchLevel.AddListener(OnLevelLaunch);
    }

    private void SetLevels()
    {
        DoOnLevels(AddLevel);
        Game.Properties.SetLevels(_levels);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Events
    private void OnLevelLoad(int pLevelIndex) => LoadLevel(pLevelIndex);
    private void OnLevelLaunch() => LaunchLevel();

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Levels
    private void DoOnLevels(Action<GameObject> pAction)
    {
        GameObject lCurrentLevel;

        for (int lLevelIndex = 0; lLevelIndex < _levelCount; lLevelIndex++)
        {
            lCurrentLevel = _transform.GetChild(lLevelIndex).gameObject;
            if (!lCurrentLevel.TryGetComponent(out Level lLevel)) continue;
            pAction(lCurrentLevel);
        }
    }

    private void UnloadLevel(GameObject pLevel)
    {
        pLevel.SetActive(false);
    }

    private void AddLevel(GameObject pLevel)
    {
        if (_levels.Contains(pLevel)) return;
        _levels.Add(pLevel);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Load
    private void UnloadAllLevels() => DoOnLevels(UnloadLevel);

    private void LoadLevel(int plevelIndex)
    {
        if (plevelIndex >= _levelCount) return;
        UnloadAllLevels();

        _currentLevel = _transform.GetChild(plevelIndex).GetComponent<Level>();
        _currentLevel.gameObject.SetActive(true);
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Level
    private void LaunchLevel()
    {
        _currentLevel.Launch();
    }
}
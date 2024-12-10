using Com.UnBocal.Rush.Properties;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JuiceManager : MonoBehaviour
{
    // Level
    private Transform _currentLevel;
    private List<JuicyCapter> _currentLevelObjects = new List<JuicyCapter>();

    // Load
    [SerializeField] private GameObject _folderFactory;
    private Transform _folder;

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Awake()
    {
        SetComponents();
        ConnectEvents();
    }

    private void Update()
    {
        
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void SetComponents()
    {

    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Events
    private void ConnectEvents()
    {
        print($"{nameof(JuiceManager)} Connects To Events");

        Game.Events.LevelLoaded.AddListener(OnLevelLoaded);
        Game.Events.End.AddListener(OnLevelEnd);
        Game.Events.LevelTouched.AddListener(OnLevelTouched);
        Game.Events.TilePlaced.AddListener(OnTilePlaced);
    }

    private void DisconnectEvent()
    {

    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Level
    private void OnLevelLoaded(Transform pLevel)
    {
        print($"{nameof(JuiceManager)} catches {pLevel.name}");

        _currentLevel = pLevel;
        SetLevelObjects();
        PlayLevelLoadAnimation();

        print(_currentLevelObjects.Count);
    }

    private void SetLevelObjects()
    {
        _currentLevelObjects.Clear();
        AddChildToLevelObject(_currentLevel);
    }

    private void AddChildToLevelObject(Transform pParent)
    {
        int lChildCount = pParent.childCount;

        if (lChildCount <= 0) return;

        if (pParent.TryGetComponent(out JuicyCapter lJuicyCapter))
        {
            _currentLevelObjects.Add(lJuicyCapter);
            return;
        }

        for (int lChildIndex = 0; lChildIndex < lChildCount; lChildIndex++)
            AddChildToLevelObject(pParent.transform.GetChild(lChildIndex));

    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Leve Load
    private void LevelLoadSetFolder()
    {
        _folder = Instantiate(_folderFactory).transform;

        _folder.position = _currentLevel.position + Vector3.down * 3f;
        _folder.localScale = Vector3.one * 5f;

        Vector3 lToCamera = Vector3.ProjectOnPlane((Camera.main.transform.position - _folder.position), Vector3.up).normalized;

        _folder.rotation *= Quaternion.LookRotation(lToCamera, Vector3.up);
    }

    private void PlayLevelLoadAnimation()
    {
        LevelLoadSetFolder();

        // Default
        float lScaleFactor = 5f;
        Vector3 lDefaultScale = Vector3.one * lScaleFactor;
        Quaternion lDefaultRotation = _folder.rotation;

        #region Spawn Folder
        // Spawn Folder
        float lSpawnTotalDuration = 1f;
        Vector3 lSpawnScaleStart = lDefaultScale.Multiply(0, 1.5f, 0);
        Vector3 lSpawnPositionEnd = _currentLevel.position + Vector3.down * 1.5f * lScaleFactor;
        Vector3 lSpawnPositionStart = lSpawnPositionEnd + Vector3.down * lScaleFactor;

        _folder.DOMove(lSpawnPositionEnd, lSpawnTotalDuration * 2f).From(lSpawnPositionStart).SetEase(Ease.OutExpo);
        _folder.DOScale(lDefaultScale, lSpawnTotalDuration * 2f).From(lSpawnScaleStart).SetEase(Ease.OutElastic);
        _folder.DOShakeRotation(lSpawnTotalDuration * 1.5f, 10f).SetEase(Ease.OutExpo);
        #endregion

        #region Launch Tiles
        // Launch
        float lPreLaunchTotalDuration = 2f;
        Vector3 lPreLaunchScale = lDefaultScale.Multiply(1.5f, .5f, 1.5f);
        Vector3 lExaggeratePreLaunchScale = lPreLaunchScale * 2f;
        Vector3 lLaunchScale = lDefaultScale.Multiply(.7f, 1.1f, .7f);
        //  // Pre Launch
        _folder.DOScale(lPreLaunchScale, lPreLaunchTotalDuration * .5f).From(lDefaultScale).SetEase(Ease.OutBack).SetDelay(lSpawnTotalDuration);
        _folder.DOShakeRotation(lPreLaunchTotalDuration * .5f, 30f).SetInverted().SetDelay(lSpawnTotalDuration);
        //  // Launch
        _folder.DOShakeRotation(2f, 15f).SetDelay(lPreLaunchTotalDuration);
        _folder.DOScale(lLaunchScale, 1f).From(lExaggeratePreLaunchScale).SetEase(Ease.OutElastic).SetDelay(lPreLaunchTotalDuration * 1.1f);
        #endregion

        #region Spawn Tiles
        // Spawn Tiles
        float lDelay = 0f;
        float pDistanceMultiplyer = .05f;
        float lTileSpawnAnimationDuration = 1f;
        float lDistanceFromCenter;

        foreach (JuicyCapter pCapter in _currentLevelObjects)
        {
            lDistanceFromCenter = (pCapter.transform.position - _currentLevel.position).magnitude;
            lDelay = lDistanceFromCenter * pDistanceMultiplyer + (lSpawnTotalDuration + lPreLaunchTotalDuration) * .75f;

            pCapter.Spawn(lTileSpawnAnimationDuration, lDelay);
        }
        #endregion

        #region Despawn Folder
        // Folder Despawn
        float lDelayIDK = 2f;
        float lFolderDespawnTotalDuration = 2f;
        Vector3 lEndScale = Vector3.zero;
        Vector3 lFoldEndRotation = Vector3.up * 5f;

        _folder.DORotate(_folder.eulerAngles, lFolderDespawnTotalDuration * .5f).SetEase(Ease.OutElastic).SetDelay(lDelay + lDelayIDK * .5f);
        _folder.DOScale(lDefaultScale, lFolderDespawnTotalDuration * .5f).SetEase(Ease.OutElastic).SetDelay(lDelay + lDelayIDK * .5f);

        _folder.DORotate(lFoldEndRotation, lFolderDespawnTotalDuration * .75f, RotateMode.Fast).SetEase(Ease.InBack).SetDelay(lDelay + lDelayIDK * .75f);

        _folder.DOMove(lSpawnPositionStart, lFolderDespawnTotalDuration).SetEase(Ease.InBack).SetDelay(lDelay + lDelayIDK * .5f);
        _folder.DOScale(lEndScale, lFolderDespawnTotalDuration * .5f).SetEase(Ease.InBack).SetDelay(lDelay + lDelayIDK);
        #endregion
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tile
    private void OnLevelTouched(Vector3 pTouchPosition)
    {
        PlayLevelWave(pTouchPosition);
    }

    private void PlayLevelWave(Vector3 pFrom)
    {
        float lDistanceFromCenter;
        foreach (JuicyCapter pCapter in _currentLevelObjects)
        {
            lDistanceFromCenter = (pCapter.transform.position - pFrom).magnitude;
            pCapter.Wave(1f, lDistanceFromCenter * .1f);
        }
    }

    private void OnTilePlaced(Transform pTile)
    {
        if (!pTile.TryGetComponent(out JuicyCapter lCapter)) return;
        lCapter.Placed();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Level End
    private void OnLevelEnd()
    {
        PlayLevelEndAnimation();
    }

    private void PlayLevelEndAnimation()
    {

    }
}

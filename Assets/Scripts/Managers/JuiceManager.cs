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

    // End
    [SerializeField] private GameObject _binFactory;
    private Transform _bin;

    // Death Indicator
    [SerializeField] private GameObject _IndicatorFactory;
    private List<JuicyCapter> _Indicators = new List<JuicyCapter>();

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
    private void Awake()
    {
        SetComponents();
        ConnectEvents();
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
    private void SetComponents()
    {

    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Events
    private void ConnectEvents()
    {
        Game.Events.LevelLoaded.AddListener(OnLevelLoaded);
        Game.Events.LevelTouched.AddListener(OnLevelTouched);
        Game.Events.TilePlaced.AddListener(OnTilePlaced);
        Game.Events.CubeDied.AddListener(CorruptOnAllLevel);

        Game.Events.CubeDied.AddListener(AddIndicator);
        Game.Events.Running.AddListener(ResetAllIndicators);
        Game.Events.StopRunning.AddListener(ResetAllIndicators);

        Game.Events.EndAnimation.AddListener(OnLevelEnd);
    }

    private void DisconnectEvent()
    {

    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Level
    private void OnLevelLoaded(Transform pLevel)
    {
        _currentLevel = pLevel;
        SetLevelObjects();
        PlayLevelLoadAnimation();
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
    private Transform SetLevelOrBin(GameObject pObjectFactory)
    {
        Transform lObject;


        lObject = Instantiate(pObjectFactory).transform;

        lObject.position = _currentLevel.position + Vector3.down * 3f;
        lObject.localScale = Vector3.one * 5f;

        Vector3 lToCamera = Vector3.ProjectOnPlane((Camera.main.transform.position - lObject.position), Vector3.up).normalized;

        lObject.rotation *= Quaternion.LookRotation(lToCamera, Vector3.up);

        return lObject;
    }

    private void PlayLevelLoadAnimation()
    {
        _folder = SetLevelOrBin(_folderFactory);

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

            pCapter.Spawn(lTileSpawnAnimationDuration, lDelay, _folder.position + _folder.up * lScaleFactor);
        }
        #endregion

        #region Despawn Folder
        // Folder Despawn
        float lDelayIDK = 2f;
        float lFolderDespawnTotalDuration = 2f;
        Vector3 lEndScale = Vector3.zero;
        Quaternion lFoldEndRotation = Quaternion.AngleAxis(180f, Vector3.up) * _folder.rotation;

        _folder.DORotate(_folder.eulerAngles, lFolderDespawnTotalDuration * .5f).SetEase(Ease.OutElastic).SetDelay(lDelay + lDelayIDK * .5f);
        _folder.DOScale(lDefaultScale, lFolderDespawnTotalDuration * .5f).SetEase(Ease.OutElastic).SetDelay(lDelay + lDelayIDK * .5f);

        _folder.DORotateQuaternion(lFoldEndRotation, lFolderDespawnTotalDuration * .75f).SetEase(Ease.InBack).SetDelay(lDelay + lDelayIDK * .75f);

        _folder.DOMove(lSpawnPositionStart, lFolderDespawnTotalDuration).SetEase(Ease.InBack).SetDelay(lDelay + lDelayIDK * .5f);
        _folder.DOScale(lEndScale, lFolderDespawnTotalDuration * .5f).SetEase(Ease.InBack).SetDelay(lDelay + lDelayIDK).onComplete = _folder.GetComponent<JuicyCapter>().Kill;
        #endregion
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Indicators
    private void ResetAllIndicators()
    {
        int lLength = _Indicators.Count;
        for (int lIndicatorIndex = lLength - 1; lIndicatorIndex >= 0; lIndicatorIndex--)
        {
            _Indicators[lIndicatorIndex].KillIndicator();
            _Indicators.RemoveAt(lIndicatorIndex);
        }
    }

    private void AddIndicator(GameObject pObject) => AddIndicator(pObject.transform.position);

    private void AddIndicator(Vector3 pPosition)
    {
        pPosition += Vector3.up * 1.5f;
        JuicyCapter lIndicator = Instantiate(_IndicatorFactory, pPosition, Quaternion.identity).GetComponent<JuicyCapter>();
        _Indicators.Add(lIndicator);
        lIndicator.SpawnIndicator();
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
            pCapter.Corrupt(1f, lDistanceFromCenter * .1f);
        }
    }

    private void OnTilePlaced(Transform pTile)
    {
        if (!pTile.TryGetComponent(out JuicyCapter lCapter)) return;
        lCapter.Placed();
    }

    private void CorruptOnAllLevel(GameObject pCube)
    {
        float lDistanceFromCenter;
        foreach (JuicyCapter pCapter in _currentLevelObjects)
        {
            lDistanceFromCenter = (pCapter.transform.position - pCube.transform.position).magnitude;
            pCapter.ShakeCubeCollision(1f, lDistanceFromCenter * .1f);
        }
    }

    // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Level End
    private void OnLevelEnd(GameObject pCube)
    {
        PlayLevelEndAnimation(pCube);
    }

    private void PlayLevelEndAnimation(GameObject pCube)
    {
        _bin = SetLevelOrBin(_binFactory);

        // Default
        float lScaleFactor = 5f;
        Vector3 lDefaultScale = Vector3.one * lScaleFactor;
        Quaternion lDefaultRotation = _bin.rotation;

        #region Spawn Bin
        // Spawn Folder
        float lSpawnTotalDuration = 1f;
        Vector3 lSpawnScaleStart = lDefaultScale.Multiply(0, 1.5f, 0);
        Vector3 lSpawnPositionEnd = _currentLevel.position + Vector3.down * 1.5f * lScaleFactor;
        Vector3 lSpawnPositionStart = lSpawnPositionEnd + Vector3.down * lScaleFactor;

        _bin.DOMove(lSpawnPositionEnd, lSpawnTotalDuration * 2f).From(lSpawnPositionStart).SetEase(Ease.OutExpo);
        _bin.DOScale(lDefaultScale, lSpawnTotalDuration * 2f).From(lSpawnScaleStart).SetEase(Ease.OutElastic);
        _bin.DOShakeRotation(lSpawnTotalDuration * 1.5f, 10f).SetEase(Ease.OutExpo);
        #endregion

        _bin.DOScale(new Vector3(1.5f, .9f, 1.5f) * lScaleFactor, 3f).SetEase(Ease.OutBack).SetDelay(lSpawnTotalDuration);

        float lDistanceFromCenter = 0f;
        foreach (JuicyCapter pCapter in _currentLevelObjects)
        {
            lDistanceFromCenter = (pCapter.transform.position - lSpawnPositionEnd).magnitude;
            pCapter.Wave(1f, lDistanceFromCenter * .1f);
            pCapter.End(1f, lSpawnTotalDuration + lDistanceFromCenter * .2f, lSpawnPositionEnd);
        }

        // Folder Despawn
        float lDelayIDK = 5f;
        float lFolderDespawnTotalDuration = 2f;
        Vector3 lEndScale = Vector3.zero;
        Quaternion lFoldEndRotation = Quaternion.AngleAxis(180f, Vector3.up) * _bin.rotation;

        _bin.DORotate(_bin.eulerAngles, lFolderDespawnTotalDuration * .5f).SetEase(Ease.OutElastic).SetDelay(lDelayIDK * .5f);
        _bin.DOScale(lDefaultScale, lFolderDespawnTotalDuration * .5f).SetEase(Ease.OutElastic).SetDelay(lDelayIDK * .5f);

        _bin.DORotateQuaternion(lFoldEndRotation, lFolderDespawnTotalDuration * .75f).SetEase(Ease.InBack).SetDelay(lDelayIDK * .75f);

        _bin.DOMove(lSpawnPositionStart, lFolderDespawnTotalDuration).SetEase(Ease.InBack).SetDelay(lDelayIDK * .5f);
        _bin.DOScale(lEndScale, lFolderDespawnTotalDuration * .5f).SetEase(Ease.InBack).SetDelay(lDelayIDK).onComplete = AnimationEndLevelFinished;
    }

    private void AnimationEndLevelFinished()
    {
        Game.Events.End.Invoke();
        _bin.GetComponent<JuicyCapter>().Kill();
    }
}

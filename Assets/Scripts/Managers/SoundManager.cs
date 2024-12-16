using Com.UnBocal.Rush.Properties;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;


    [SerializeField] private AudioSource _MainMusic;
    [SerializeField] private AudioSource _CubeRolling;
    [SerializeField] private AudioSource _CubeWall;
    [SerializeField] private AudioSource _CubeActionTile;
    [SerializeField] private AudioSource _CubeGoal;
    [SerializeField] private AudioSource _Crash;


    private float _pitchModifier = .2f;

    private void Awake()
    {
        ConnectEvents();
    }

    private void ConnectEvents()
    {
        Game.Events.PlayCubeRolling.AddListener(OnPlayCubeRolling);
        Game.Events.PlayCubeWallCollision.AddListener(OnPlayCubeWallCollision);
        Game.Events.PlayCubeActionTile.AddListener(OnPlayCubeActionTile);
        Game.Events.PlayCubeGoal.AddListener(OnPlayCubeGoal);
        Game.Events.CubeDied.AddListener(OnCubeDies);
        Game.Events.StopRunning.AddListener(ResetMusic);
        Game.Events.Running.AddListener(ResetMusic);
    }

    private void OnPlayCubeRolling()
    {
        _CubeRolling.pitch = 1 + _pitchModifier - Random.value * _pitchModifier * .5f;
        _CubeRolling.Play();
    }

    private void OnPlayCubeWallCollision()
    {
        _CubeWall.pitch = 1 + _pitchModifier - Random.value * _pitchModifier * .5f;
        _CubeWall.Play();
    }

    private void OnPlayCubeActionTile()
    {
        _CubeActionTile.pitch = 1 + _pitchModifier - Random.value * _pitchModifier * .5f;
        _CubeActionTile.Play();
    }

    private void OnPlayCubeGoal()
    {
        _CubeGoal.pitch = 1 + _pitchModifier - Random.value * _pitchModifier * .5f;
        _CubeGoal.Play();
    }

    private void ResetMusic()
    {
        _audioMixer.FindSnapshot("Default").TransitionTo(.5f);
    }

    private void OnCubeDies(GameObject pCube)
    {
        _Crash.Play();
        _audioMixer.FindSnapshot("Error").TransitionTo(.5f);
    }
}

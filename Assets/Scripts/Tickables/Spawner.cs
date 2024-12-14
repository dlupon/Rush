using Com.UnBocal.Rush.Properties;
using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEngine;


namespace Com.UnBocal.Rush.Tickables
{
    public class Spawner : Tickable
    {
        // Components
        [SerializeField] private Transform _transformRenderer;
        private GameObject _gameObject;
        private Typable _typable;
        private Collider _collider;
        private Rolling _rolling;
        private CollisionDetector _collisionDetector;

        // Tick
        private Action OnTick;

        // Materials
        [SerializeField] private Material _materialColors;

        // Spawner
        [SerializeField] private GameObject _cubeFactory;
        [SerializeField] private int _spawnLoop = 1;
        [SerializeField][Tooltip("0 -> No Spawn\n1 -> Forward\n2 -> Right\n3 -> Back\n4 -> Left")] private int[] _spawningRate;
        [SerializeField] private bool _isCube = false;
        private int _spawningCount = 0;
        private int _spawningLoopCount = 0;

        // Recovery
        private Vector3 _recoveryPosition;
        private Quaternion _recoveryRotation;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
        protected override void OnStart()
        {
            SetRecoveryProperties();
            SetPause();
        }

        protected override void SetComponents()
        {
            base.SetComponents();
            _gameObject = gameObject;
            _typable = GetComponent<Typable>();
            _rolling = GetComponent<Rolling>();
            _collisionDetector = GetComponent<CollisionDetector>();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tick
        #region Tick
        protected override void Tick() => OnTick();

        private void Spawn()
        {
            if (_spawningLoopCount > _spawnLoop || _spawningLoopCount >= _spawningRate.Length) { StopSpawning(); return; }
            if (_spawningRate[_spawningCount] != 0)
            {
                if (_spawningCount == _spawningRate.Length - 1 && _isCube) { SpawnCube(SetAsCube()); return; }
                else SpawnCube();
            }

            if (++_spawningCount >= _spawningRate.Length) { _spawningLoopCount++; _spawningCount = 0; }
            WaitFor(2);
        }

        private void Wait() { }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Spawn
        #region Spawn
        private void SpawnCube(Transform pCurrentCube)
        {
            Vector3 lDirection = Quaternion.AngleAxis(Game.Properties.ROTATION * (_spawningRate[_spawningCount] - 1), Vector3.up) * m_transform.forward;
            pCurrentCube.GetComponent<Rolling>().SetDirection(lDirection);
        }

        private void SpawnCube()
        {
            Transform _currentCube = Instantiate(_cubeFactory, m_transform.position, transform.rotation).transform;
            _currentCube.GetComponent<Typable>().SetCubeType(_typable.CubeType);

            SpawnCube(_currentCube);
        }


        private Transform SetAsCube()
        {
            _gameObject.layer = Game.Properties.LayerCube;

            _rolling.ResetMovementProperties();

            StopSpawning();

            return m_transform;
        }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // On Running
        #region On Running
        protected override void OnStopRunning() { SetPause(); print("SetPause"); }

        protected override void OnRunning() { SetSpawn(); print("SetSpawn"); }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Set States
        #region Set States
        private void SetPause()
        {
            Recovery();
            OnTick = Wait;
        }

        private void SetSpawn()
        {
            m_transform.DOKill();

            Recovery();

            _gameObject.layer = Game.Properties.LayerIgnore;
            _spawningCount = 0;
            _spawningLoopCount = 0;

            OnTick = Spawn;
        }

        private void StopSpawning() => OnTick = Wait;

        private void SetCubeBehaviorOn(bool pOn)
        {
            return;
            _rolling.enabled = pOn;
            _rolling.SetListening(pOn);
            _collisionDetector.enabled = pOn;
            _collisionDetector.SetListening(pOn);
        }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Recovery
        #region Recovery
        private void SetRecoveryProperties()
        {
            _recoveryPosition = m_transform.position;
            _recoveryRotation = m_transform.rotation;
        }

        private void Recovery()
        {
            m_transform.position = _recoveryPosition;
            m_transform.rotation = _recoveryRotation;
        }
        #endregion
    }
}
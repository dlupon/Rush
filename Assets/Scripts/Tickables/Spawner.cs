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
        private GameObject _gameObject;
        private Typable _typable;
        private Collider _collider;
        private Rolling _rolling;
        private CollisionDetector _collisionDetector;
        private JuicyCapter _juicyCapter;

        // Tick
        private Action OnTick;

        // Materials
        [SerializeField] private Material _materialColors;

        // Spawner
        [SerializeField] private GameObject _cubeSpawnerFactory;
        [SerializeField] private GameObject _cubeFactory;
        [SerializeField] private int _spawnLoop = 1;
        [SerializeField][Tooltip("0 -> No Spawn\n1 -> Forward\n2 -> Right\n3 -> Back\n4 -> Left")] private int[] _spawningRate;
        [SerializeField] private bool _lastCubeSpawner = true;
        private int _lastCubeIndex = -1;
        private int _spawningCount = 0;
        private int _spawningLoopCount = 0;
        private int _totalCubeCount = 0;

        // Recovery
        private Vector3 _recoveryPosition;
        private Quaternion _recoveryRotation;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
        protected override void OnStart()
        {
            SetSpawnProperties();
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
            _juicyCapter = GetComponent<JuicyCapter>();
        }

        private void SetSpawnProperties()
        {
            for (int lLoopIndex = 0; lLoopIndex < _spawnLoop; lLoopIndex++)
                foreach (int lCubeIndex in _spawningRate)
                {
                    if (lCubeIndex == 0) continue;
                    _lastCubeIndex++;
                }
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tick
        #region Tick
        protected override void Tick() => OnTick();

        private void Spawn()
        {
            if (_totalCubeCount > _lastCubeIndex) { StopSpawning(); return; }
            if (_spawningRate[_spawningCount] != 0)
            {
                if (_lastCubeSpawner && _lastCubeIndex <= _totalCubeCount) { SpawnCube(SetSpawnerCube()); return; }
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
            _totalCubeCount++;
            Vector3 lDirection = Quaternion.AngleAxis(Game.Properties.ROTATION * (_spawningRate[_spawningCount] - 1), Vector3.up) * m_transform.forward;
            pCurrentCube.GetComponent<Rolling>().SetDirection(lDirection);
        }

        private void SpawnCube()
        {
            Transform _currentCube = Instantiate(_cubeFactory, m_transform.position, transform.rotation).transform;
            _currentCube.GetComponent<Typable>().SetCubeType(_typable.CubeType);

            SpawnCube(_currentCube);
        }


        private Transform SetSpawnerCube()
        {
            Transform _currentCube = Instantiate(_cubeSpawnerFactory, m_transform.position, transform.rotation).transform;
            _currentCube.GetComponent<Typable>().SetCubeType(_typable.CubeType);

            gameObject.SetActive(false);

            // StopSpawning();

            return _currentCube;
        }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // On Running
        #region On Running
        protected override void OnStopRunning() { SetPause(); }

        protected override void OnRunning() { SetSpawn(); }
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
            _totalCubeCount = 0;

            OnTick = Spawn;
        }

        private void StopSpawning() => OnTick = Wait;
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
            gameObject.SetActive(true);
            m_transform.position = _recoveryPosition;
            m_transform.rotation = _recoveryRotation;
        }
        #endregion
    }
}
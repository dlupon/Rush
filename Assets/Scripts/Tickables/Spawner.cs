using Com.UnBocal.Rush.Properties;
using DG.Tweening;
using System;
using UnityEngine;


namespace Com.UnBocal.Rush.Tickables
{
    public class Spawner : Tickable
    {
        // Components
        [SerializeField] private Transform _transformRenderer;
        [SerializeField] private MeshRenderer _meshRenderer;
        private GameObject _gameObject;
        private Collider _collider;

        // Tick
        private Action OnTick;

        // Materials
        [SerializeField] private Material _materialColors;

        // Spawner
        [SerializeField] private GameObject _cubeFactory;
        [SerializeField] [Tooltip("0 -> No Spawn\n1 -> Forward\n2 -> Right\n3 -> Back\n4 -> Left")] private int[] _spawningRate;
        [SerializeField] private bool _isCube = false;
        private int _spawningCount = 0;

        // Collision
        [SerializeField] private LayerMask _cubeLayer = default;
        [SerializeField] private LayerMask _sideLayer = default;
        [SerializeField] private LayerMask _groundLayer = default;
        private LayerMask _defaultLayer = default;

        // Recovery
        private Vector3 _recoveryPosition;
        private Quaternion _recoveryRotation;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
        protected override void OnStart()
        {
            GetComponent<ChangeColorMaterial>().SetMaterialColor(_materialColors);
            SetRecoveryProperties();
            Setpause();
        }

        protected override void SetComponents()
        {
            base.SetComponents();
            _gameObject = gameObject;
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tick
        #region Tick
        protected override void Tick() => OnTick();

        private void Spawn()
        {
            if (_spawningCount >= _spawningRate.Length) { StopSpawning();  return; }
            if (_spawningRate[_spawningCount] != 0)
            {
                if (_spawningCount == _spawningRate.Length - 1 && _isCube) SpawnCube(SetAsCube());
                else SpawnCube();
            }
            _spawningCount++;
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

            if (pCurrentCube.TryGetComponent(out ChangeColorMaterial lCurrentChangeColorMaterial))
                lCurrentChangeColorMaterial.SetMaterialColor(_materialColors);
        }

        private void SpawnCube()
        {
            Transform _currentCube = Instantiate(_cubeFactory, m_transform.position, transform.rotation).transform;
            SpawnCube(_currentCube);
        }


        private Transform SetAsCube()
        {
            m_transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);

            CollisionDetector _CollisionComponent = _gameObject.AddComponent<CollisionDetector>();
            Rolling _rollingComponent = _gameObject.AddComponent<Rolling>();
            _rollingComponent.SetRenderer(_transformRenderer);
            _gameObject.layer = Game.Properties.LayerCube;
            _CollisionComponent.SetLayer(_cubeLayer, _sideLayer, _groundLayer);

            _CollisionComponent.SetDeleteOnStopRunning(true);
            _rollingComponent.SetDeleteOnStopRunning(true);
            StopSpawning();
            
            return m_transform;
        }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // On Running
        #region On Running
        protected override void OnStopRunning() => Setpause();

        protected override void OnRunning() => SetSpawn();
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Set States
        #region Set States
        private void Setpause()
        {
            Recovery();
            OnTick = Wait;
        }

        private void SetSpawn()
        {
            m_transform.position = _recoveryPosition;
            m_transform.rotation = _recoveryRotation;
            _gameObject.layer = Game.Properties.LayerIgnore;
            _spawningCount = 0;
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
            m_transform.DOMove(_recoveryPosition, 1f).SetEase(Ease.OutExpo);
            m_transform.DORotate(_recoveryRotation.eulerAngles, 1f).SetEase(Ease.OutExpo);
        }
        #endregion
    }
}
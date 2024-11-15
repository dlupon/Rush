using DG.Tweening;
using System;
using UnityEngine;
using Com.UnBocal.Rush.Properties;
using UnityEngine.Events;


namespace Com.UnBocal.Rush.Tickables
{
    public class Spawner : Tickable
    {
        // Components
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private Transform _transformRenderer;
        [SerializeField] private MeshRenderer _meshRenderer;

        // Materials
        [SerializeField] private Material _materialColors;

        // Spawner
        [SerializeField] private GameObject _cubeFactory;
        [SerializeField] [Tooltip("0 -> No Spawn\n1 -> Forward\n2 -> Right\n3 -> Back\n4 -> Left")] private int[] _spawningRate;
        [SerializeField] private bool _isCube = false;
        private int _spawningCount = 0;

        protected override void OnStart()
        {
            GetComponent<ChangeColorMaterial>().SetMaterialColor(_materialColors);
        }

        protected override void SetComponents()
        {
            base.SetComponents();
            _gameObject = gameObject;
        }

        protected override void Tick() => Spawn();

        private void Spawn()
        {
            if (_spawningCount >= _spawningRate.Length) return;
            if (_spawningRate[_spawningCount] != 0)
            {
                if (_spawningCount == _spawningRate.Length - 1 && _isCube) SetAsCube();
                else SpawnCube();
            }
            _spawningCount++;
        }

        private void SpawnCube()
        {
            Transform _currentCube = Instantiate(_cubeFactory, transform.position, transform.rotation).transform;
            _currentCube.rotation = Quaternion.AngleAxis(90f * (_spawningRate[_spawningCount] - 1), Vector3.up) * m_transform.rotation;
            _currentCube.GetComponent<ChangeColorMaterial>().SetMaterialColor(_materialColors);
        }
    
        private void SetAsCube()
        {
            Rolling _rollingComponent = _gameObject.AddComponent<Rolling>();
            _rollingComponent.SetRenderer(_transformRenderer);
            Destroy(this);
        }
    }
}
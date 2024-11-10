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
        [SerializeField] private Transform _transformRenderer;
        [SerializeField] private MeshRenderer _meshRenderer;

        // Materials
        [SerializeField] private Material _materialColors;

        // Spawner
        [SerializeField] private GameObject _cubeFactory;
        [SerializeField] [Tooltip("0 -> No Spawn\n1 -> Forward\n2 -> Right\n3 -> Back\n4 -> Left")] private int[] _spawningRate;
        [SerializeField] private bool _isCube = false;
        private int _spawningCount = 0;

        private void Start()
        {
            GetComponent<ChangeColorMaterial>().SetMaterialColor(_materialColors);
        }

        protected override void LateTick() => Spawn();

        private void Spawn()
        {
            if (_spawningCount >= _spawningRate.Length) return;
            if (_spawningRate[_spawningCount] != 0)
            {
                Transform _currentCube = Instantiate(_cubeFactory).transform;
                _currentCube.position = m_transform.position;
                _currentCube.rotation = Quaternion.AngleAxis(90f * (_spawningRate[_spawningCount] - 1), Vector3.up) * m_transform.rotation;
                _currentCube.GetComponent<ChangeColorMaterial>().SetMaterialColor(_materialColors);
            }
            _spawningCount++;
        }
    }
}
using DG.Tweening;
using System;
using UnityEngine;
using Com.UnBocal.Rush.Properties;
using UnityEngine.Events;
using UnityEngine.UIElements;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.UIElements;


namespace Com.UnBocal.Rush.Tickables
{
    [RequireComponent(typeof(CollisionDetector))]
    public class Rolling : Tickable
    {
        public Vector3 Direction { get => _direction; }

        // Components
        [SerializeField] private Transform _transformRenderer = null;
        private CollisionDetector _collisionDetector = null;

        // Animation
        [SerializeField] private AnimationCurve _curveMove = null;

        // Stats
        private Action OnUpdate;
        private Action OnTick;

        // Rotation
        [SerializeField] private int _lineNumber = 10;
        public const float ROTATION = 90f;
        private Vector3 _rotationAxis;

        private Quaternion _startRotation = Quaternion.identity;
        private Quaternion _endRotation = Quaternion.identity;

        private Quaternion _startRotationPosition;
        private Quaternion _endRotationPosition;
        // Movements
        private Vector3 _direction = Vector3.forward;
        private Vector3 _startPosition, _endPosition = Vector3.zero;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
        private void Start()
        {
            _endPosition = m_transform.position;
            _direction = m_transform.forward;
            SetCurve();
            SetMove();
        }

        private void Update() => OnUpdate();
        
        protected override void SetComponents()
        {
            base.SetComponents();
            if (_transformRenderer == null) _transformRenderer = m_transform;
            _collisionDetector = GetComponent<CollisionDetector>();
        }

        protected override void ConnectEvents()
        {
            _collisionDetector.CollisionWall.AddListener(WallCollision);
            _collisionDetector.CollisionArrow.AddListener(WallCollision);
            _collisionDetector.CollisionCube.AddListener(CubeCollision);
            _collisionDetector.Stuck.AddListener(SetStuck);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Events

        protected override void Tick() => OnTick();

        private void CubeCollision()
        {
            SetSpawn();
        }

        private void OnArrowCollision(Vector3 pDirection)
        {
            ChangeOrientation(pDirection);
            SetMove();
        }

        private void WallCollision(Vector3 pDirection) => SetStartMove(pDirection);

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Set States
        private void SetSpawn()
        {
            OnUpdate = UpdateStuck;
            OnTick = TickSpawn;
        }

        private void SetStartMove() => SetStartMove(m_transform.forward);

        private void SetStartMove(Vector3 pDirection)
        {
            WaitFor(2);
            _direction = pDirection;
            OnUpdate = UpdateStuck;
            OnTick = TickStartMove;
        }

        private void SetMove()
        {
            ChangeOrientation();
            UpdateRotationProperties();
            TickMove();
            OnTick = TickMove;
            OnUpdate = UpdateMove;
        }

        private void SetStuck()
        {
            OnUpdate = UpdateStuck;
            OnTick = UpdateStuck;
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Update States

        private void UpdateMove()
        {
            Vector3 l_position = -(_direction + Vector3.down).normalized * Mathf.Sqrt(2) * .5f;
            bool l_ratioOutofBounds = Game.Properties.TickRatio >= 1f;
            float l_ratio = l_ratioOutofBounds ? 1 : _curveMove.Evaluate(Game.Properties.TickRatio);
            float l_PositionMultiplyer = Mathf.Floor(l_ratio);
            Vector3 l_PositionOffset = _direction * ((l_ratioOutofBounds ? 0 : l_PositionMultiplyer) + .5f);

            Quaternion l_rotationCube = Quaternion.LerpUnclamped(_startRotation, _endRotation, l_ratio);

            Quaternion l_rotationPosition = l_ratio < 0 ?
                Quaternion.LerpUnclamped(_endRotationPosition, _startRotationPosition, -l_ratio) :                  // Under 0
                l_ratio > 1 ? Quaternion.LerpUnclamped(_startRotationPosition, _endRotationPosition, l_ratio - 1) : // Above 1
                Quaternion.Lerp(_startRotationPosition, _endRotationPosition, l_ratio);                             // Between 0 And 1

            _transformRenderer.rotation = l_rotationCube;
            _transformRenderer.position = _startPosition + l_PositionOffset + l_rotationPosition * l_position;

            Debug.DrawRay(_startPosition + l_PositionOffset, l_rotationPosition * l_position * 3, Color.red);
        }

        private void UpdateStuck() { }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tick States

        private void TickSpawn()
        {
            
        }

        private void TickStartMove() => SetMove();

        private void TickMove()
        {
            ChangeOrientation(_direction);
            _rotationAxis = Quaternion.AngleAxis(ROTATION, Vector3.up) * _direction * ROTATION;
            UpdateNextPositionAndRotation();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Utilities

        private void UpdateRotationProperties()
        {
            _startRotation = _endRotation;
            _endRotation = Quaternion.AngleAxis(ROTATION, _rotationAxis) * _startRotation;

            _startRotationPosition = Quaternion.identity;
            _endRotationPosition = Quaternion.AngleAxis(ROTATION, _rotationAxis) * _startRotationPosition;
        }

        private void UpdateNextPositionAndRotation()
        {
            _startPosition = _endPosition;
            _endPosition = _startPosition + _direction;

            UpdateRotationProperties();

            m_transform.position = _endPosition;
        }

        private void ChangeOrientation(Vector3 pDirection)
        {
            _direction = pDirection.normalized;
            Quaternion rotation = _transformRenderer.rotation;
            m_transform.rotation = Quaternion.LookRotation(pDirection.normalized);
            _transformRenderer.rotation = rotation;
        }

        private void ChangeOrientation() => ChangeOrientation(_direction);

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Setters

        private void SetCurve()
        {
            if (_curveMove != null) return;
            _curveMove = new AnimationCurve();
            _curveMove.AddKey(0, 0);
            _curveMove.AddKey(1, 1);
        }

        public void SetRenderer(Transform pTransform) => _transformRenderer = pTransform;
    }
}
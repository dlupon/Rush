using System;
using UnityEngine;
using Com.UnBocal.Rush.Utilities;
using Com.UnBocal.Rush.Properties;

namespace Com.UnBocal.Rush.Tickables
{
    [RequireComponent(typeof(CollisionDetector))]
    public class Rolling : Tickable
    {
        public Vector3 Direction { get => _direction; }
        public Vector3 ConveyorDirection { get => _conveyorDirection; }

        // Components
        [SerializeField] private Transform _transformRenderer = null;
        private CollisionDetector _collisionDetector = null;

        // Animation
        [SerializeField] private AnimationCurve _curveMove = null;

        // Stats
        private Action OnUpdate;
        private Action OnTick;

        // Rotation
        public const float ROTATION = 90f;
        private Vector3 _rotationAxis;

        private Quaternion _startRotation = Quaternion.identity;
        private Quaternion _endRotation = Quaternion.identity;

        private Quaternion _startRotationPosition;
        private Quaternion _endRotationPosition;
        // Movements
        private Vector3 _direction = Vector3.forward;
        private Vector3 _conveyorOffset = Vector3.up * .5f;
        private Vector3 _conveyorDirection = Vector3Int.forward;
        private Vector3 _startPosition, _endPosition = Vector3Int.zero;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity

        private void Update() => OnUpdate();

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization

        #region Initialization
        protected override void OnStart()
        {
            SetDefaultMotionProperties();
            SetCurve();
            SetStartMove();
        }

        protected override void SetComponents()
        {
            base.SetComponents();
            if (_transformRenderer == null) _transformRenderer = m_transform;
            _collisionDetector = GetComponent<CollisionDetector>();
        }

        protected override void ConnectEvents()
        {
            _collisionDetector.CollisionWall.AddListener(OnWallCollision);
            _collisionDetector.CollisionCube.AddListener(OnCubeCollision);
            _collisionDetector.CollisionArrow.AddListener(OnArrowCollision);
            _collisionDetector.CollisionStopper.AddListener(OnStopperCollision);
            _collisionDetector.CollisionConveyor.AddListener(OnConveyorCollision);
            _collisionDetector.Stuck.AddListener(SetStuck);
            _collisionDetector.OutCollisionConveyor.AddListener(OnConveyorOut);
        }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Events

        #region Events
        protected override void Tick() => OnTick();

        private void OnCubeCollision() => SetSpawn();

        private void OnArrowCollision(Vector3 pDirection) => ChangeDirection(pDirection);

        private void OnWallCollision(Vector3 pDirection) => SetStartMove(pDirection);

        private void OnStopperCollision() => SetStartMove();

        private void OnConveyorCollision(Vector3 pDirection) => SetConveyor(pDirection);

        private void OnConveyorOut() => SetStartMove();
        #endregion
        
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Set States
        
        #region Set States
        private void SetSpawn() { OnUpdate = UpdateStuck; OnTick = TickSpawn; }

        private void SetStartMove() => SetStartMove(_direction);

        private void SetStartMove(Vector3 pDirection)
        {
            m_tickListerner.WaitFor(1);

            ChangeDirection(pDirection);
            OnTick = SetMove;
            OnUpdate = UpdateStuck;
        }

        private void SetMove()
        {
            UpdateNextPositionAndRotation();
            OnTick = TickMove;
            OnUpdate = UpdateMove;
        }

        private void SetConveyor(Vector3 pDirection)
        {
            _conveyorDirection = pDirection.Round();
            OnTick = TickConveyor;
            OnUpdate = UpdateConveyor;
        }

        private void SetStuck() => SetStuck(0);

        private void SetStuck(int pWaitTick)
        {
            WaitFor(pWaitTick);
            OnUpdate = UpdateStuck;
            OnTick = SetStartMove;
        }
        #endregion
        
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Update States
        
        #region Updates Stats
        private void UpdateMove()
        {
            // l_positionOffset = Offset Position Of The Cube From The Rotation Axis

            // We Need All Thoses Fixes Because The Animation Curve Could Give You A Value Under 0 Or Above 1
            // l_ratioOutofBounds = Used To Fix Isues Created By The Ratio When It's Above Or Equase to 1
            // l_ratio = Fixed Ratio + Modified By The Animation Cuve
            // l_PositionMultiplyer = Determines How Much The Cube Is Offseted From The End Position

            // l_PositionOffset = Fixes The Position Based On The Animation Curve

            // l_rotationPosition 1 => Rotation That Can Go Under 0 Without Weird Visual (Animation Curve)
            //                    2 => Rotation That Can Go Above 1 Without Weird Visual (Animation Curve)
            //                    3 => Default Rotation

            Vector3 l_positionOffset = -(_direction + Vector3.down).normalized * Mathf.Sqrt(2) * .5f;
            bool l_ratioOutofBounds = Game.Properties.TickRatio >= 1f;
            float l_ratio = l_ratioOutofBounds ? 1 : _curveMove.Evaluate(Game.Properties.TickRatio);
            float l_PositionMultiplyer = Mathf.Floor(l_ratio);
            Vector3 l_PositionOffset = _direction * ((l_ratioOutofBounds ? 0 : l_PositionMultiplyer) + .5f);

            // Lerp Cube Position
            Quaternion l_rotationCube = Quaternion.LerpUnclamped(_startRotation, _endRotation, l_ratio);
            // Lerp Cube Rotation
            Quaternion l_rotationPosition = l_ratio < 0 ?
                Quaternion.LerpUnclamped(_endRotationPosition, _startRotationPosition, -l_ratio) :                  // Under 0
                l_ratio > 1 ? Quaternion.LerpUnclamped(_startRotationPosition, _endRotationPosition, l_ratio - 1) : // Above 1
                Quaternion.Lerp(_startRotationPosition, _endRotationPosition, l_ratio);                             // Between 0 And 1

            // Apply Lerps
            _transformRenderer.rotation = l_rotationCube;
            _transformRenderer.position = _startPosition + l_PositionOffset + l_rotationPosition * l_positionOffset;

            // Debug
            Debug.DrawRay(_startPosition + l_PositionOffset, l_rotationPosition * l_positionOffset * 3, Color.red);
        }

        private void UpdateConveyor()
        {
            _transformRenderer.position = _conveyorOffset + Vector3.Lerp(_startPosition, _endPosition, Game.Properties.TickRatio);
        }

        private void UpdateStuck() { }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tick States

        #region Tick States
        private void TickSpawn() { }

        private void TickStartMove() => SetMove();

        private void TickMove() => UpdateNextPositionAndRotation();

        private void TickConveyor() => UpdatenextPosition(_conveyorDirection);

        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Utilities

        #region Utilities
        private void UpdatenextPosition(Vector3 pDirection)
        {
            // Update Position
            _startPosition = _endPosition;
            _endPosition = _startPosition + pDirection.Round();

            // Apply The Movement
            m_transform.position = _endPosition;
        }

        private void UpdateNextRotation()
        {
            // Update Rotation
            _startRotation = _endRotation;
            _endRotation = Quaternion.AngleAxis(ROTATION, _rotationAxis) * _startRotation;

            // Update Rotation For The Position Offset
            _startRotationPosition = Quaternion.identity;
            _endRotationPosition = Quaternion.AngleAxis(ROTATION, _rotationAxis) * _startRotationPosition;
        }

        private void UpdateNextPositionAndRotation() => UpdateNextPositionAndRotation(_direction);

        private void UpdateNextPositionAndRotation(Vector3 pDirection)
        {
            UpdatenextPosition(pDirection.normalized);
            UpdateNextRotation();
        }

        private void ChangeDirection() => ChangeDirection(_direction);

        private void ChangeDirection(Vector3 pDirection)
        {
            _direction = pDirection.normalized;
            _rotationAxis = Quaternion.AngleAxis(ROTATION, Vector3.up) * _direction;
        }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Setters

        #region Setters
        private void SetDefaultMotionProperties()
        {
            _endPosition = _startPosition = m_transform.position.Round(); ;
            _endRotation = _startRotation = _transformRenderer.rotation;
            _direction = m_transform.forward;
        }

        private void SetCurve()
        {
            if (_curveMove != null) return;
            _curveMove = new AnimationCurve();
            _curveMove.AddKey(0, 0);
            _curveMove.AddKey(1, 1);
        }

        public void SetRenderer(Transform pTransform) => _transformRenderer = pTransform;
        #endregion
    }
}
using System;
using UnityEngine;
using Com.UnBocal.Rush.Properties;
using DG.Tweening;
using UnityEngine.Assertions.Must;
using TMPro;

namespace Com.UnBocal.Rush.Tickables
{
    public class Rolling : Tickable
    {
        public Vector3 Direction { get => _direction; }
        public Vector3 ConveyorDirection { get => _conveyorDirection; }

        // Components
        [SerializeField] private Transform _transformRenderer = null;
        private CollisionDetector _collisionDetector = null;

        // Animation
        [SerializeField] private AnimationCurve _curveMove = null;
        [SerializeField] private AnimationCurve _curveTeleportationIn = null;
        [SerializeField] private AnimationCurve _curveTeleportationOut = null;
        private AnimationCurve _curveCurrent = null;

        // Stats
        private Action OnUpdate;
        private Action OnTick;

        // Rotation
        private Vector3 _rotationAxis;

        private Quaternion _startRotation = Quaternion.identity;
        private Quaternion _endRotation = Quaternion.identity;

        private Quaternion _startRotationPosition;
        private Quaternion _endRotationPosition;

        // Scale
        private Vector3 _startScale = Vector3.one;
        private Vector3 _endScale = Vector3.one;

        // Movements
        private Vector3 _direction = Vector3.zero;
        private Vector3 _conveyorOffset = Vector3.up * .5f;
        private Vector3 _conveyorDirection = Vector3Int.forward;
        private Vector3 _startPosition, _endPosition = Vector3Int.zero;

        // Tick Count
        private const int TICK_START_MOVE = 1;
        private const int TICK_COLLISION_WITH_WALL = 2;
        private const int TICK_COLLISION_WITH_STOP = 1;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Update / Tick
        private void Update() => OnUpdate();

        protected override void Tick() => OnTick();

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
        #region Initialization
        protected override void OnStart()
        {
            SetDefaultMotionProperties();
            SetCurve();
            SetSpawn();
        }

        protected override void SetComponents()
        {
            base.SetComponents();
            if (_transformRenderer == null) _transformRenderer = m_transform;
            _collisionDetector = GetComponent<CollisionDetector>();
        }

        protected override void ConnectEvents()
        {
            base.ConnectEvents();
            _collisionDetector.CollisionWall.AddListener(OnWallCollision);
            _collisionDetector.CollisionCube.AddListener(OnCubeCollision);
            _collisionDetector.CollisionArrow.AddListener(OnArrowCollision);
            _collisionDetector.CollisionStopper.AddListener(OnStopperCollision);
            _collisionDetector.CollisionTeleporter.AddListener(OnTeleporterCollision);
            _collisionDetector.CollisionConveyor.AddListener(OnConveyorCollision);
            _collisionDetector.Stuck.AddListener(SetStuck);
            _collisionDetector.OutCollisionConveyor.AddListener(OnConveyorOut);
            _collisionDetector.Falling.AddListener(OnFalling);
            _collisionDetector.Land.AddListener(OnLand);
        }

        protected override void DisconnectEvent()
        {
            base.DisconnectEvent();
            _collisionDetector.CollisionWall.RemoveListener(OnWallCollision);
            _collisionDetector.CollisionCube.RemoveListener(OnCubeCollision);
            _collisionDetector.CollisionArrow.RemoveListener(OnArrowCollision);
            _collisionDetector.CollisionStopper.RemoveListener(OnStopperCollision);
            _collisionDetector.CollisionTeleporter.RemoveListener(OnTeleporterCollision);
            _collisionDetector.CollisionConveyor.RemoveListener(OnConveyorCollision);
            _collisionDetector.Stuck.RemoveListener(SetStuck);
            _collisionDetector.OutCollisionConveyor.RemoveListener(OnConveyorOut);
            _collisionDetector.Falling.RemoveListener(OnFalling);
            _collisionDetector.Land.RemoveListener(OnLand);
        }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Events
        #region Events
        private void OnCubeCollision()
        {
            _transformRenderer.DOKill();
            _transformRenderer.DOShakeRotation(1f, 20f).SetEase(Ease.OutExpo);
        }

        private void OnArrowCollision(Vector3 pDirection) => ChangeDirection(pDirection);

        private void OnWallCollision(Vector3 pDirection) => SetStartMoveAfterWall(pDirection);

        private void OnStopperCollision() => SetStuck(TICK_COLLISION_WITH_STOP);

        private void OnTeleporterCollision(Vector3 p_newPosition) => SetTeleportation(p_newPosition);

        private void OnConveyorCollision(Vector3 pDirection) => SetConveyor(pDirection);

        private void OnConveyorOut() => SetStartMove();

        private void OnFalling() => SetFalling();

        private void OnLand() => SetStartMove();
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Set States
        #region Set States
        private void SetSpawn()
        {
            Game.Properties.CubeSpawn();
            OnUpdate = UpdateStuck;
            OnTick = SetStartMove;
        }

        private void SetStartMove() => SetStartMove(_direction);

        private void SetStartMove(Vector3 pDirection)
        {
            m_tickListerner.WaitFor(TICK_START_MOVE);

            ChangeDirection(pDirection);
            OnTick = SetMove;
            OnUpdate = UpdateStuck;
        }

        private void SetStartMoveAfterWall(Vector3 pDirection)
        {
            m_tickListerner.WaitFor(TICK_COLLISION_WITH_WALL);

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
            m_tickListerner.ResetWaitTick();
            _conveyorDirection = pDirection.Round();
            OnTick = TickConveyor;
            OnUpdate = UpdateSlide;
        }

        private void SetStuck() => SetStuck(0);

        private void SetStuck(int pWaitTick)
        {
            m_tickListerner.WaitFor(pWaitTick);
            OnUpdate = UpdateStuck;
            OnTick = SetStartMove;
        }

        private void SetFalling()
        {
            OnTick = TickFalling;
            OnUpdate = UpdateSlide;
        }

        private void SetTeleportation(Vector3 p_newPosition)
        {
            ChangeEndPotition(p_newPosition);

            OnTick = TickTeleportationOut;
            OnUpdate = UpdateTeleportation;

            m_transform.position = _startPosition + Vector3.up * .5f;

            _curveCurrent = _curveTeleportationIn;

            _startScale = Vector3.one;
            _endScale = Vector3.zero;
        }

        #endregion
        
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Update States
        #region Updates Stats
        private void UpdateMove()
        {
            // l_PositionOffsetFromRotationAxis = Offset Position Of The Cube From The Rotation Axis

            // We Need All Thoses Fixes Because The Animation Curve Could Give You A Value Under 0 Or Above 1
            // l_ratioOutofBounds = Used To Fix Isues Created By The Ratio When It's Above Or Equase to 1
            // l_ratio = Fixed Ratio + Modified By The Animation Cuve
            // l_PositionMultiplyer = Determines How Much The Cube Is Offseted From The End Position

            // l_PositionOffset = Fixes The Position Based On The Animation Curve

            // l_rotationPosition 1 => Rotation That Can Go Under 0 Without Weird Visual (Animation Curve)
            //                    2 => Rotation That Can Go Above 1 Without Weird Visual (Animation Curve)
            //                    3 => Default Rotation

            Vector3 l_PositionOffsetFromRotationAxis = -(_direction + Vector3.down).normalized * Mathf.Sqrt(2) * .5f;
            bool l_ratioOutofBounds = Game.Properties.TickRatio >= 1f;
            float l_ratio = l_ratioOutofBounds ? 1 : _curveMove.Evaluate(Game.Properties.TickRatio);
            float l_PositionMultiplyer = Mathf.Floor(l_ratio);
            Vector3 l_PositionOffset = _direction * ((l_ratioOutofBounds ? 0 : l_PositionMultiplyer) + .5f);

            // Lerp Cube Rotation
            // Lerp Cube Rotation
            Quaternion l_rotationCube = Quaternion.LerpUnclamped(_startRotation, _endRotation, l_ratio);
            // Lerp Cube Position 
            Quaternion l_rotationPosition = l_ratio < 0 ?
                Quaternion.LerpUnclamped(_endRotationPosition, _startRotationPosition, -l_ratio) :                  // Under 0
                l_ratio > 1 ? Quaternion.LerpUnclamped(_startRotationPosition, _endRotationPosition, l_ratio - 1) : // Above 1
                Quaternion.Lerp(_startRotationPosition, _endRotationPosition, l_ratio);                             // Between 0 And 1

            // Apply Lerps 
            m_transform.rotation = l_rotationCube;
            m_transform.position = _startPosition + l_PositionOffset + l_rotationPosition * l_PositionOffsetFromRotationAxis;

            // Debug
            Debug.DrawRay(_startPosition + l_PositionOffset, l_rotationPosition * l_PositionOffsetFromRotationAxis, Color.red);
        }

        private void UpdateSlide()
        {
            m_transform.position = _conveyorOffset + Vector3.Lerp(_startPosition, _endPosition, Game.Properties.TickRatio);
        }

        private void UpdateTeleportation()
        {
            float lRatio = _curveCurrent.Evaluate(Game.Properties.TickRatio);
            transform.localScale = Vector3.LerpUnclamped(_startScale, _endScale, lRatio);
        }

        private void UpdateStuck() { }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tick States
        #region Tick States
        private void TickSpawn() { }

        private void TickStartMove() => SetMove();

        private void TickMove() => UpdateNextPositionAndRotation();

        private void TickConveyor() => UpdateNextPosition(_conveyorDirection);

        private void TickFalling() => UpdateNextPosition(Vector3.down);

        private void TickTeleportationOut()
        {
            OnTick = SetStartMove;
            m_transform.position = _endPosition + Vector3.up * .5f;

            _curveCurrent = _curveTeleportationIn;

            _startScale = Vector3.zero;
            _endScale = Vector3.one;
        }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Utilities
        #region Utilities
        private void UpdateNextPosition(Vector3 pDirection)
        {
            // Update Position
            _startPosition = _endPosition;
            _endPosition = _startPosition + pDirection.Round();

            m_transform.position = _startPosition + Vector3.up * .5f;

            // Debug
            Debug.DrawLine(_startPosition, _endPosition, Color.green, 1f);
        }

        private void UpdateNextRotation()
        {
            // Update Rotation
            _startRotation = _endRotation;
            _endRotation = Quaternion.AngleAxis(Game.Properties.ROTATION, _rotationAxis) * _startRotation;

            // Update Rotation For The Position Offset
            _startRotationPosition = Quaternion.identity;
            _endRotationPosition = Quaternion.AngleAxis(Game.Properties.ROTATION, _rotationAxis) * _startRotationPosition;
        }

        private void UpdateNextPositionAndRotation() => UpdateNextPositionAndRotation(_direction);

        private void UpdateNextPositionAndRotation(Vector3 pDirection)
        {
            UpdateNextPosition(pDirection.normalized);
            UpdateNextRotation();
        }

        private void ChangeDirection() => ChangeDirection(_direction);

        private void ChangeDirection(Vector3 pDirection)
        {
            _direction = pDirection.normalized;
            _rotationAxis = Quaternion.AngleAxis(Game.Properties.ROTATION, Vector3.up) * _direction;
        }

        private void ChangeEndPotition(Vector3 p_newPosition)
        {
            _startPosition = _endPosition;
            _endPosition = p_newPosition;
        }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Setters
        #region Setters

        public void ResetMovementProperties()
        {
            _direction = Vector3.zero;
            OnStart();
            SetStartMove();
        }

        private void SetDefaultMotionProperties()
        {
            _endPosition = _startPosition = m_transform.position + Vector3.down * .5f;
            _endRotation = _startRotation = m_transform.rotation;
            _direction = _direction == Vector3.zero ? m_transform.forward : _direction;
        }

        private void SetCurve()
        {
            if (_curveMove != null) return;
            _curveMove = new AnimationCurve();
            _curveMove.AddKey(0, 0);
            _curveMove.AddKey(1, 1);
        }

        public void SetRenderer(Transform pTransform) => _transformRenderer = pTransform;

        public void SetDirection(Vector3 pDirection) => _direction = pDirection;
        #endregion
    }
}
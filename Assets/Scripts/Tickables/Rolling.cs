using DG.Tweening;
using System;
using UnityEngine;
using Com.UnBocal.Rush.Properties;
using UnityEngine.Events;
using UnityEngine.UIElements;


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
        [SerializeField] private Ease _easeStartMoving = Ease.InBack;
        [SerializeField] private Ease _easeMoving = Ease.Linear;

        // Motion Properties
        private Action MotionAction;
        // Rotation
        public const float ROTATION = 90f;
        private float _positionCorrector = 0f;
        private Vector3 _rotationAxis;
        // Movements
        private Vector3 _direction = Vector3.forward;
        private Vector3 _startPosition, _endPosition = Vector3.zero;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
        private void Start()
        {
            _positionCorrector = (Mathf.Sqrt(2) - 1) * .5f;
            _endPosition = Game.Properties.WorldGrid.WorldToCell(m_transform.position);
            SetSpawn();
        }

        private void Update() => CorrectY();

        protected override void SetComponents()
        {
            base.SetComponents();
            if (_transformRenderer == null) _transformRenderer = m_transform;
            _collisionDetector = GetComponent<CollisionDetector>();
        }

        protected override void ConnectEvents()
        {
            _collisionDetector.CollisionWall.AddListener(WallCollision);
            _collisionDetector.CollisionCube.AddListener(CubeCollision);
            _collisionDetector.Stuck.AddListener(SetStuck);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Ticking
        // protected override void FirstTick() => MotionAction();

        protected override void LateTick() => MotionAction();

        private void FixedUpdate()
        {
            Debug.DrawLine( Vector3.up * .75f + _startPosition, Vector3.up * .75f + _endPosition, Color.black, .1f);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Events
        private void CubeCollision()
        {
            SetSpawn();
        }

        private void WallCollision(Vector3 pDirection)
        {
            SetStartMove(pDirection);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Set States
        private void SetSpawn() => MotionAction = Spawn;

        private void SetStartMove() => SetStartMove(m_transform.forward);

        private void SetStartMove(Vector3 pDirection)
        {
            _direction = pDirection;
            MotionAction = StartMove;
        }

        private void SetMove() => MotionAction = Move;

        private void SetStuck() => MotionAction = Stuck;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // States
        private void Spawn()
        {
            m_tickListerner.WaitFor(2);

            Vector3 l_scale = m_transform.localScale;
            float l_duration = Game.Properties.TickInterval * m_tickListerner.TickLeft;
            _transformRenderer.DOKill();
            _transformRenderer.DOScale(l_scale, l_duration).From(Vector3.zero).SetEase(Ease.OutElastic);

            SetStartMove();

            SetText(nameof(Spawn));
        }

        private void StartMove()
        {
            UpdatePositionsAndDirection(_direction);

            // m_tickListerner.WaitFor(2);
            WaitFor(2);

            float duration = Game.Properties.TickInterval * m_tickLeft;

            _transformRenderer.DOKill(); m_transform.DOKill();
            _transformRenderer.DORotate(_rotationAxis, duration, RotateMode.WorldAxisAdd).SetEase(_easeStartMoving);
            m_transform.DOMove(_endPosition, duration).SetEase(_easeStartMoving);

            SetText(nameof(StartMove));

            SetMove();
        }

        private void Move()
        {
            UpdatePositionsAndRotation();

            _transformRenderer.DOKill(); m_transform.DOKill();
            _transformRenderer.DORotate(_rotationAxis, Game.Properties.TickInterval, RotateMode.WorldAxisAdd).SetEase(_easeMoving);
            m_transform.DOMove(_endPosition, Game.Properties.TickInterval).SetEase(_easeMoving);

            SetText(nameof(Move));
        }

        private void Stuck() { SetText(nameof(Stuck)); }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Utilities
        private void CorrectY()
        {
            float x = (m_transform.position - _endPosition).magnitude;
            Vector3 lCorrector = Vector3.up * _positionCorrector * Mathf.Abs(Mathf.Sin(x * Mathf.PI));
            _transformRenderer.position = m_transform.position + Vector3.up * .5f + lCorrector;
        }

        private void UpdatePositionsAndRotation()
        {
            _startPosition = _endPosition;
            _endPosition = _startPosition + _direction;
        }

        private void UpdatePositionsAndDirection(Vector3 pDirection)
        {
            _direction = pDirection.normalized;
            ChangeOrientation(_direction);
            _rotationAxis = Quaternion.AngleAxis(ROTATION, Vector3.up) * _direction * ROTATION;
            UpdatePositionsAndRotation();
        }

        private void ChangeOrientation(Vector3 pDirection)
        {
            Quaternion rotation = _transformRenderer.rotation;
            m_transform.rotation = Quaternion.LookRotation(pDirection.normalized);
            _transformRenderer.rotation = rotation;
        }

        private void test()
        {

        }
    }
}
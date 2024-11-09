using DG.Tweening;
using System;
using UnityEngine;
using Com.UnBocal.Rush.Properties;


namespace Com.UnBocal.Rush.Tickables
{
    public class Rolling : Tickable
    {
        // Components
        [SerializeField] private Transform _transformRenderer = null;

        // Animation
        [SerializeField] private float _duration = 1f;
        [SerializeField] private Ease _easeStartMoving = Ease.InBack;
        [SerializeField] private Ease _easeMoving = Ease.Linear;
        private float _currentDuration = 1f;

        // Motion Properties
        private Action MotionAction;
        // Rotation
        private const float ROTATION = 90f;
        private float _positionCorrector = 0f;
        private Vector3 _rotationAxis;
        // Movements
        private Vector3 _direction = Vector3.forward;
        private Vector3 _startPosition, _endPosition = Vector3.zero;

        private void Update() => CorrectY();

        protected override void SetComponents()
        {
            base.SetComponents();
            if (_transformRenderer == null) _transformRenderer = _transform;
        }

        protected override void OnAwake()
        {
            // Set Properties
            _positionCorrector = (Mathf.Sqrt(2) - 1) * .5f;
        }

        protected override void FirstTick() => ChangeDirectionAndMove(_transform.forward);

        protected override void Tick()
        {
            if (--m_tickLeft > 0) return;
            MotionAction();
        }

        private void Move()
        {
            UpdatePositionsAndRotation();

            _transformRenderer.DOKill(); _transform.DOKill();
            _transformRenderer.DORotate(_rotationAxis, Game.Properties.Speed, RotateMode.WorldAxisAdd).SetEase(_easeMoving);
            _transform.DOMove(_endPosition, Game.Properties.Speed).SetEase(_easeMoving);
        }

        private void CorrectY()
        {
            float x = (_transform.position - _endPosition).magnitude;
            Vector3 lCorrector = Vector3.up * _positionCorrector * Mathf.Abs(Mathf.Sin(x * Mathf.PI));
            _transformRenderer.position = _transform.position + Vector3.up * .5f + lCorrector;
        }

        private void ChangeDirectionAndMove(Vector3 pDirection)
        {
            UpdatePositionsAndDirection(pDirection);

            _transformRenderer.DOKill(); _transform.DOKill();
            _transformRenderer.DORotate(_rotationAxis, Game.Properties.Speed * m_tickLeft, RotateMode.WorldAxisAdd).SetEase(_easeStartMoving);
            _transform.DOMove(_endPosition, Game.Properties.Speed * m_tickLeft).SetEase(_easeStartMoving);
        }

        private void UpdatePositionsAndRotation()
        {
            _startPosition = _transformRenderer.position;
            _endPosition = _transform.position + _direction;
        }

        private void UpdatePositionsAndDirection(Vector3 pDirection)
        {
            _direction = pDirection.normalized;
            _rotationAxis = Quaternion.AngleAxis(ROTATION, Vector3.up) * _direction * ROTATION;
            UpdatePositionsAndRotation();

            m_tickLeft = 2;
            MotionAction = Move;
        }
    }
}
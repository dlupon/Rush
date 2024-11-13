using Com.UnBocal.Rush.Properties;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.WSA;


namespace Com.UnBocal.Rush.Tickables
{
    public class CollisionDetector : Tickable
    {
        // Event
        public UnityEvent<Vector3> CollisionWall = new UnityEvent<Vector3>();
        public UnityEvent<Vector3> CollisionArrow = new UnityEvent<Vector3>();
        public UnityEvent CollisionCube = new UnityEvent();
        public UnityEvent Stuck = new UnityEvent();

        // Components
        private Collider _collider;
        private Rolling _rolling;

        // Collosion
        private const float _DEBUG_RAY_DURATION = .1f;
        private const float RAYCAST_LENGTH = 1f;
        private bool _collisionDetected = false;
        private RaycastHit _hit;
        private Vector3 _direction;
        private Vector3 _groundDirection = Vector3.down;
        private Vector3 _offset = Vector3.up * .5f;

        // Debug
        private static int _colorCount = 0;
        private int _colorIndex = 0;
        private Color[] _colors = new Color[5]
        {
            Color.cyan,
            Color.green,
            Color.blue,
            Color.yellow,
            Color.red,
        };
        private Color _color;

        protected override void OnAwake()
        {
            base.OnAwake();
            _colorIndex = _colorCount++;
            _color = _colors[_colorIndex];
        }

        protected override void SetComponents()
        {
            base.SetComponents();
            _collider = GetComponent<Collider>();
            _rolling = GetComponent<Rolling>();
        }

        protected override void Tick()
        {
            if (CheckCollisionGround()) return;
            CheckCollisionWallAndBlocs();
        }

        private bool CheckCollisionGround()
        {
            LaunchRaycast(_groundDirection);
            if (!_collisionDetected) return true;
            if (_hit.collider.tag.Contains("Arrow"))
            {
                CollisionArrow.Invoke(_hit.collider.transform.forward);
                return true;
            }
            return false;
        }

        public void CheckCollisionWallAndBlocs()
        {
            _direction = GetComponent<Rolling>().Direction;
            int l_currentDrectionIndex;
            int l_maxDirectionIndex = 4;

            for (l_currentDrectionIndex = 0; l_currentDrectionIndex  < l_maxDirectionIndex; l_currentDrectionIndex++)
            {
                LaunchRaycast(_direction, _color);

                if (!_collisionDetected)
                {
                    if (l_currentDrectionIndex <= 0) return;
                    Debug.DrawLine(m_transform.position, m_transform.position + _direction * 5f, Color.magenta, 4f);
                    CollisionWall.Invoke(_direction);
                    return;
                }

                if (_hit.collider.tag != "Tile")
                {
                    CollisionCube.Invoke();
                    Time.timeScale = 0;
                    _hit.collider.transform.position += Vector3.up;
                    return;
                }

                _direction = Quaternion.AngleAxis(Rolling.ROTATION, Vector3.up) * _direction;
            }

            if (l_currentDrectionIndex >= l_maxDirectionIndex) Stuck.Invoke();
        }

        private void LaunchRaycast(Vector3 pDirection, Color pColor)
        {
            Vector3 l_position = m_transform.position + _offset;
            Debug.DrawLine(l_position, l_position + pDirection * RAYCAST_LENGTH, pColor, _DEBUG_RAY_DURATION);
            _collisionDetected = Physics.Raycast(l_position, pDirection, out _hit, RAYCAST_LENGTH);
        }

        private void LaunchRaycast(Vector3 pDirection) => LaunchRaycast(pDirection, Color.red);
    }
}
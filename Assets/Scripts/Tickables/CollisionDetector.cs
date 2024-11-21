using Com.UnBocal.Rush.Properties;
using DG.Tweening;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;


namespace Com.UnBocal.Rush.Tickables
{
    public class CollisionDetector : Tickable
    {
        // Event
        public UnityEvent<Vector3> CollisionWall = new UnityEvent<Vector3>();
        public UnityEvent<Vector3> CollisionArrow = new UnityEvent<Vector3>();
        public UnityEvent<Vector3> CollisionTeleporter = new UnityEvent<Vector3>();
        public UnityEvent CollisionSwitch = new UnityEvent();
        public UnityEvent CollisionStopper = new UnityEvent();
        public UnityEvent<Vector3> CollisionConveyor = new UnityEvent<Vector3>();
        public UnityEvent OutCollisionConveyor = new UnityEvent();
        public UnityEvent CollisionCube = new UnityEvent();
        public UnityEvent Stuck = new UnityEvent();
        public UnityEvent Falling = new UnityEvent();
        public UnityEvent Land = new UnityEvent();

        // Components
        private Collider _collider;
        private Rolling _rolling;

        // Collosion
        private const float _DEBUG_RAY_DURATION = 1f;
        private const float RAYCAST_LENGTH = 1f;
        private bool _collisionDetected = false;
        private RaycastHit _hit;
        private Vector3 _direction;
        private Vector3 _groundDirection = Vector3.down;
        private Vector3 _offset = Vector3.up * .5f;
        private bool _isGrounded = true;
        private bool _isOnConveyor = false;


        // Positions For Switch Tile
        private Vector3 _lastPosition = default;

        protected override void SetComponents()
        {
            base.SetComponents();
            _collider = GetComponent<Collider>();
            _rolling = GetComponent<Rolling>();
        }

        protected override void Tick()
        {
            CheckCollision();
            UpdateCubePosition();
        }

        private void CheckCollision()
        {
            CheckGroundCollisions();
            if (!_isGrounded) return;
            CheckCollisionWallAndBlocs();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Ground Collision
        #region Ground Collision
        private void CheckGroundCollisions()
        {
            if (!GroundCollision()) return;
            if (CubeDidntMove()) return;
            // Cube Behavior Based on The Collisiond
            switch (_hit.collider.tag)
            {
                case "Arrow": OnCollisionArrow(); return;
                case "Switch": OnCollisionSwitch(); return;
                case "Stopper": OnCollisionStopper(); return;
                case "Conveyor": OnCollisionConveyor(); return;
                case "Teleporter": OnCollisionTeleporter(); return;
                default: ResetGroundProperties(); return;
            }
        }

        private bool GroundCollision()
        {
            _isGrounded = LaunchRaycast(_groundDirection);

            print(_isGrounded);

            if (!_isGrounded) Falling.Invoke();
            else if ((_lastPosition - m_transform.position).y > 0f) Land.Invoke();
            return _isGrounded;
        }

        private void OnCollisionArrow()
        {
            ResetGroundProperties();
            // Don't Launch The Event If The Arrow Don't Change The Initial Direction
            if (_hit.collider.transform.forward == _rolling.Direction) return;
            CollisionArrow.Invoke(_hit.collider.transform.forward);
        }

        private void OnCollisionSwitch()
        {
            ResetGroundProperties();
            if (!_hit.collider.TryGetComponent(out Switch l_switch)) return;
            Quaternion l_rotation = Quaternion.AngleAxis(Rolling.ROTATION * l_switch.Orientation, Vector3.up);
            CollisionArrow.Invoke(l_rotation * _rolling.Direction);
        }

        private void OnCollisionStopper()
        {
            ResetGroundProperties();
            CollisionStopper.Invoke();
        }

        private void OnCollisionConveyor()
        {
            _isOnConveyor = true;
            CollisionConveyor.Invoke(_hit.collider.transform.forward);
        }
        
        private void OnCollisionTeleporter()
        {
            if (CubeDidntMove()) return;
            CollisionTeleporter.Invoke(_hit.collider.GetComponent<Teleporter>().TeleportPosition);
        }

        private void ResetGroundProperties()
        {
            if (!_isOnConveyor) return;
            _isOnConveyor = false;
            OutCollisionConveyor.Invoke();
        }

        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Side Collision
        #region Side Collision
        public void CheckCollisionWallAndBlocs()
        {
            if (ConveyorCollisionCheck()) return;
            DefaultCollisionCheck();
        }

        private bool ConveyorCollisionCheck()
        {
            if (!_isOnConveyor) return false;
            _direction = _rolling.ConveyorDirection;
            return !LaunchRaycast(_direction);
        }

        private void DefaultCollisionCheck()
        {
            ResetGroundProperties();

            _direction = _rolling.Direction;
            int l_currentDrectionIndex;
            int l_maxDirectionIndex = 4;

            // Check Collision In 4 Directions
            for (l_currentDrectionIndex = 0; l_currentDrectionIndex < l_maxDirectionIndex; l_currentDrectionIndex++)
            {
                LaunchRaycast(_direction, Color.red);
                // Can The Cube Go In This Direction
                if (NoCollisionFound(l_currentDrectionIndex)) return;
                else if (CollideWithBlocs()) return;

                _direction = Quaternion.AngleAxis(Rolling.ROTATION, Vector3.up) * _direction;
            }

            if (l_currentDrectionIndex >= l_maxDirectionIndex) Stuck.Invoke();
        }

        private bool NoCollisionFound(int pDirectionIndex)
        {
            if (_collisionDetected) return false;
            // Don't Launch The Event If The Cube Has Nothing On It's Way
            if (pDirectionIndex <= 0 || _isOnConveyor) return true;
            // Reset Conveyor Movement

            CollisionWall.Invoke(_direction);
            return true;
        }

        private bool CollideWithBlocs()
        {
            if (_hit.collider.tag == "Tile") return false;

            CollisionCube.Invoke();
            return true;
        }

        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Motion Properties
        #region Motion Properties
        private void UpdateCubePosition() => _lastPosition = m_transform.position;

        private bool CubeDidntMove() => _lastPosition == m_transform.position;
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Raycast
        #region Raycasts
        private bool LaunchRaycast(Vector3 pDirection, Color pColor)
        {
            _hit = default;
            Vector3 l_position = m_transform.position + _offset;
            // Debug
            Debug.DrawLine(l_position, l_position + pDirection * RAYCAST_LENGTH, pColor, _DEBUG_RAY_DURATION);

            return _collisionDetected = Physics.Raycast(l_position, pDirection, out _hit, RAYCAST_LENGTH);
        }

        private bool LaunchRaycast(Vector3 pDirection) => LaunchRaycast(pDirection, Color.red);
        #endregion
    }
}
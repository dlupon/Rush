using Com.UnBocal.Rush.Properties;
using UnityEngine;
using UnityEngine.Events;

namespace Com.UnBocal.Rush.Tickables
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class CollisionDetector : Tickable
    {
        // Event
        [HideInInspector] public UnityEvent<Vector3> CollisionWall = new UnityEvent<Vector3>();
        [HideInInspector] public UnityEvent<Vector3> CollisionArrow = new UnityEvent<Vector3>();
        [HideInInspector] public UnityEvent<Vector3> CollisionTeleporter = new UnityEvent<Vector3>();
        [HideInInspector] public UnityEvent CollisionSwitch = new UnityEvent();
        [HideInInspector] public UnityEvent CollisionStopper = new UnityEvent();
        [HideInInspector] public UnityEvent<Vector3> CollisionConveyor = new UnityEvent<Vector3>();
        [HideInInspector] public UnityEvent OutCollisionConveyor = new UnityEvent();
        [HideInInspector] public UnityEvent CollisionCube = new UnityEvent();
        [HideInInspector] public UnityEvent Stuck = new UnityEvent();
        [HideInInspector] public UnityEvent Falling = new UnityEvent();
        [HideInInspector] public UnityEvent Land = new UnityEvent();

        // Components
        private JuicyCapter _juicyCapter;
        private BoxCollider _collider;
        private Rigidbody _body;
        private Rolling _rolling;
        private Typable _typable;

        // Collosion
        [SerializeField] private LayerMask _GroundLayer;
        [SerializeField] private LayerMask _SideLayer;
        [SerializeField] private LayerMask _CubeLayer;
        private const float FALLING_DISTANCE_MIN = .5f;
        private const float DEBUG_RAY_DURATION = 1f;
        private const float RAYCAST_LENGTH = 1f;
        private bool _collisionDetected = false;
        private RaycastHit _hit;
        private Vector3 _direction;
        private Vector3 _groundDirection = Vector3.down;
        private bool _needSideCheck = false;
        private bool _isGrounded = false;
        private bool _isOnConveyor = false;
        private bool _justTeleported = false;
        private int _fallingTick = 0;

        // Positions
        private bool _CubeDidntMove => _lastPosition == m_transform.position;
        private Vector3 _lastPosition = default;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
        #region Initialization
        protected override void SetComponents()
        {
            base.SetComponents();
            _juicyCapter = GetComponent<JuicyCapter>();
            _collider = GetComponent<BoxCollider>();
            _body = GetComponent<Rigidbody>();
            _rolling = GetComponent<Rolling>();
            _typable = GetComponent<Typable>();
        }
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tick
        protected override void Tick()
        {
            CheckCollision();
            UpdateCubePosition();
        }

        private void CheckCollision()
        {
            CheckGroundCollisions();
            if (!(_isGrounded && _needSideCheck && !_justTeleported)) return;
            CheckSideCollision();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Ground Collision
        #region Ground Collision
        private void CheckGroundCollisions()
        {
            _needSideCheck = true;
            if (!GroundCollision()) return;
            if (_CubeDidntMove) return;
            // Cube Behavior Based on The Collisiond
            switch (_hit.collider.tag)
            {
                case "Goal": OnCollisionGoal(); return;
                case "Arrow": OnCollisionArrow(); break;
                case "Switch": OnCollisionSwitch(); break;
                case "Stopper": OnCollisionStopper(); break;
                case "Conveyor": OnCollisionConveyor(); break;
                case "Teleporter": OnCollisionTeleporter(); break;
                default: ResetGroundProperties(); return;
            }

            Game.Events.PlayCubeActionTile.Invoke();
        }

        private bool GroundCollision()
        {
            _isGrounded = LaunchRaycast(_groundDirection, _GroundLayer);

            if (!_isGrounded) IsFalling();
            else
            {
                if (Mathf.Abs((_lastPosition - m_transform.position).y) > FALLING_DISTANCE_MIN) 
                {
                    _juicyCapter.Shake();
                    Land.Invoke();
                }
                if (!_CubeDidntMove)
                {
                    Game.Events.PlayCubeRolling.Invoke();
                    JuiceOnCollider();
                }
                _fallingTick = 0;
            }
            return _isGrounded;
        }

        private void OnCollisionGoal()
        {
            if (!_hit.collider.TryGetComponent(out Typable _GoalTypable)) return;
            if (_GoalTypable.CubeType != _typable.CubeType) return;

            _juicyCapter.CubeGoalAnimation(_hit.collider.gameObject);

            _collider.enabled = false;
            gameObject.layer = Game.Properties.LayerIgnore;

            m_tickListerner.SetTicking(false);
            Stuck.Invoke();

            Game.Properties.CubeDies(gameObject);

            Game.Events.PlayCubeGoal.Invoke();
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
            Quaternion l_rotation = Quaternion.AngleAxis(Game.Properties.ROTATION * l_switch.Orientation, Vector3.up);
            CollisionArrow.Invoke(l_rotation * _rolling.Direction);
        }

        private void OnCollisionStopper()
        {
            ResetGroundProperties();
            _needSideCheck = false;
            CollisionStopper.Invoke();
        }

        private void OnCollisionConveyor()
        {
            _isOnConveyor = true;
            CollisionConveyor.Invoke(_hit.collider.transform.forward);
        }
        
        private void OnCollisionTeleporter()
        {
            ResetGroundProperties();
            if (_justTeleported)
            {
                _justTeleported = false;
                return;
            }
            m_tickListerner.WaitFor(1);
            _needSideCheck = false;
            _justTeleported = true;
            CollisionTeleporter.Invoke(_hit.collider.GetComponent<Teleporter>().TeleportPosition);
        }

        private void ResetGroundProperties()
        {
            if (!_isOnConveyor) return;
            _isOnConveyor = false;
            OutCollisionConveyor.Invoke();
        }

        private void IsFalling()
        {
            Falling.Invoke();
            if (++_fallingTick <= 3) return;
            if (LaunchRaycast(Vector3.down, _GroundLayer, 50f)) return;
            Game.Events.CubeDied.Invoke(gameObject);
            Game.Events.CubeDiedFromFalling.Invoke();
        }

        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Side Collision
        #region Side Collision
        public void CheckSideCollision()
        {
            if (ConveyorCollisionCheck()) return;
            DefaultCollisionCheck();
        }

        private bool ConveyorCollisionCheck()
        {
            if (!_isOnConveyor) return false;
            _direction = _rolling.ConveyorDirection;
            return !LaunchRaycast(_direction, _SideLayer);
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
                LaunchRaycast(_direction, _SideLayer);
                // Can The Cube Go In This Direction
                if (NoCollisionFound(l_currentDrectionIndex)) return;

                _direction = Quaternion.AngleAxis(Game.Properties.ROTATION, Vector3.up) * _direction;
            }

            if (l_currentDrectionIndex >= l_maxDirectionIndex) Stuck.Invoke();
        }

        private bool NoCollisionFound(int pDirectionIndex)
        {
            if (_collisionDetected) return false;
            // Don't Launch The Event If The Cube Has Nothing On It's Way
            if (pDirectionIndex <= 0 || _isOnConveyor)
            {
                CollisionArrow.Invoke(_direction);
                return true;
            }

            CollisionWall.Invoke(_direction);
            _juicyCapter.Shake();

            Game.Events.PlayCubeWallCollision.Invoke();

            return true;
        }

        private bool CollideWithBlocs()
        {
            if (_hit.collider.tag == "Tile") return false;

            CollisionCube.Invoke();
            return true;
        }

        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Cube Collision
        #region Cube Collision

        private void OnTriggerEnter(Collider other)
        {
            Game.Events.CubeDied.Invoke(gameObject);
            Game.Events.CubeDiedFromCollision.Invoke();
        }

        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Motion Properties
        #region Motion Properties
        private void UpdateCubePosition() => UpdateCubePosition(m_transform.position);
        private void UpdateCubePosition(Vector3 pNewPosition) => _lastPosition = pNewPosition;
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Raycast
        #region Raycasts

        private bool LaunchRaycast(Vector3 pDirection, LayerMask pLayer, Color pColor, float pDistance = RAYCAST_LENGTH)
        {
            _hit = default;
            Vector3 l_position = m_transform.position;
            // Debug
            Debug.DrawLine(l_position, l_position + pDirection * pDistance, pColor, DEBUG_RAY_DURATION);

            return _collisionDetected = Physics.Raycast(l_position, pDirection, out _hit, pDistance, pLayer);
        }

        private bool LaunchRaycast(Vector3 pDirection, LayerMask pLayer, float pDistance = RAYCAST_LENGTH) => LaunchRaycast(pDirection, pLayer, Color.red, pDistance);
        #endregion


        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Juice
        private void JuiceOnCollider(int pIteration = 1)
        {
            if (_hit.collider.gameObject.tag == "Goal") return;
            if (!_hit.collider.TryGetComponent(out JuicyCapter lJT)) return;
            lJT.Shake();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Setter
        public void SetLayer(LayerMask pCubeLayer, LayerMask pSideLayer, LayerMask pGroundLayer)
        {
            _CubeLayer = pCubeLayer;
            _SideLayer = pSideLayer;
            _GroundLayer = pGroundLayer;
        }
    }
}
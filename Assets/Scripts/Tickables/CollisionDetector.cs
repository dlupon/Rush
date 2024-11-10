using Com.UnBocal.Rush.Properties;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


namespace Com.UnBocal.Rush.Tickables
{
    public class CollisionDetector : Tickable
    {
        // Event
        public UnityEvent<Vector3> CollisionWall = new UnityEvent<Vector3>();
        public UnityEvent CollisionCube = new UnityEvent();
        public UnityEvent Stuck = new UnityEvent();

        // Components
        private Collider _collider;
        private Rolling _rolling;

        // Collosion
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

        protected override void Tick() => CheckCollision();

        public void CheckCollision()
        {
            bool l_collisionDetected = default;
            RaycastHit l_hit = default;
            Vector3 l_direction = GetComponent<Rolling>().Direction;
            int l_currentDrectionIndex;
            int l_maxDirectionIndex = 4;

            for (l_currentDrectionIndex = 0; l_currentDrectionIndex  < l_maxDirectionIndex; l_currentDrectionIndex++)
            {
                l_collisionDetected = LunchRaycast(m_transform.position + _offset, l_direction, 1f, out l_hit, _color, .1f);

                if (!l_collisionDetected)
                {
                    if (l_currentDrectionIndex <= 0) return;
                    Debug.DrawLine(m_transform.position, m_transform.position + l_direction * 5f, Color.magenta, Game.Properties.TickInterval * 4);
                    CollisionWall.Invoke(l_direction);
                    return;
                }

                if (l_hit.collider.tag != "Tile")
                {
                    CollisionCube.Invoke();
                    Time.timeScale = 0;
                    l_hit.collider.transform.position += Vector3.up;
                    return;
                }

                l_direction = Quaternion.AngleAxis(Rolling.ROTATION, Vector3.up) * l_direction;
            }

            if (l_currentDrectionIndex >= l_maxDirectionIndex) Stuck.Invoke();
        }

        private bool LunchRaycast(Vector3 pPos, Vector3 pDirection, float pLength, out RaycastHit pHitInfo, Color pColor = default, float pDuration = 0f)
        {
            Debug.DrawLine(pPos, pPos + pDirection * pLength, pColor, pDuration);
            return Physics.Raycast(pPos, pDirection, out pHitInfo, pLength);
        }
    }
}
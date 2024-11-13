using UnityEngine;
using Com.UnBocal.Rush.Debugs;
using Unity.IO.LowLevel.Unsafe;

namespace Com.UnBocal.Rush.Tickables
{
    [RequireComponent(typeof(TickListener))]
    public class Tickable : MonoBehaviour
    {
        // Components
        protected TickListener m_tickListerner;
        protected Transform m_transform;
        protected DebugStateMachin m_debugTools = null;

        // Ticks
        protected int m_tickLeft { get => _tickLeft; }
        private int _tickLeft = 0;


        private void Awake()
        {
            SetComponents();
            SetTickEvents();
            ConnectEvents();
            OnAwake();
        }

        protected virtual void SetComponents() { m_tickListerner = GetComponent<TickListener>(); TryGetComponent(out m_debugTools); m_transform = transform; }
        private void SetTickEvents()
        {
            m_tickListerner.LocalTick.AddListener(OnTick);
        }

        protected virtual void OnAwake() { }
        protected virtual void ConnectEvents() { }
        private void OnTick()
        {
            if (--_tickLeft > 0) return;
            Tick();
        }
        protected virtual void Tick()
        {
        
        }
        protected virtual void LateTick() { }

        protected void SetText(string ptext)
        {
            if (m_debugTools == null) return;
            m_debugTools.SetText(ptext);
        }

        protected void WaitFor(int pTickNumber) => _tickLeft = pTickNumber;
    }
}
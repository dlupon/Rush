using UnityEngine;
using Com.UnBocal.Rush.Debugs;

namespace Com.UnBocal.Rush.Tickables
{
    [RequireComponent(typeof(TickListener))]
    public class Tickable : MonoBehaviour
    {
        // Components
        protected TickListener m_tickListerner;
        protected Transform m_transform;
        protected DebugStateMachin m_debugTools = null;


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
            m_tickListerner.LocalTick.AddListener(Tick);
            m_tickListerner.LocalLateTick.AddListener(LateTick);
        }

        protected virtual void OnAwake() { }
        protected virtual void ConnectEvents() { }
        protected virtual void Tick() { }
        protected virtual void LateTick() { }

        protected void SetText(string ptext)
        {
            if (m_debugTools == null) return;
            m_debugTools.SetText(ptext);
        }
    }
}
using UnityEngine;
using Com.UnBocal.Rush.Debugs;
using Com.UnBocal.Rush.Properties;

namespace Com.UnBocal.Rush.Tickables
{
    [RequireComponent(typeof(EventListener))]
    public class Tickable : MonoBehaviour
    {
        // Components
        protected EventListener m_tickListerner;
        protected Transform m_transform;
        protected DebugStateMachin m_debugTools = null;

        // Ticks
        public bool IsTicking => _isTicking;
        private bool _isTicking = true;
        protected int m_tickLeft => _tickLeft;
        private int _tickLeft = 0;

        // Running
        [SerializeField] protected bool m_deleteOnStopRunning = false;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
        private void Start()
        {
            SetComponents();
            SetTickEvents();
            ConnectEvents();
            OnStart();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
        protected virtual void SetComponents()
        {
            m_tickListerner = GetComponent<EventListener>();
            m_transform = transform; }
        private void SetTickEvents()
        {
            m_tickListerner.LocalTick.AddListener(OnTick);
        }

        protected virtual void OnStart() { }

        protected virtual void ConnectEvents()
        {
            Game.Events.Running.AddListener(OnRunning);
            Game.Events.StopRunning.AddListener(PreStopRunning);
        }

        protected virtual void DisconnectEvent()
        {
            Game.Events.Running.RemoveListener(OnRunning);
            Game.Events.StopRunning.RemoveListener(PreStopRunning);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tick
        private void OnTick()
        {
            if (!_isTicking) return;
            if (!(m_tickListerner.IsTicking && m_tickListerner.TickLeft <= 0)) return;
            if (--_tickLeft > 0) return;
            Tick();
        }

        protected virtual void Tick() { }

        protected void WaitFor(int pTickNumber) => _tickLeft = pTickNumber;

        public void SetListening(bool pListening) => _isTicking = pListening;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Game Running
        protected virtual void OnRunning()
        {

        }

        private void PreStopRunning()
        {
            OnStopRunning();
            Delete();
        }

        protected virtual void OnStopRunning()
        {

        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Delete
        private void Delete()
        {
            if (!m_deleteOnStopRunning) return;
            Destroy(this);
        }

        public void SetDeleteOnStopRunning(bool pDelete) => m_deleteOnStopRunning = pDelete;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // 
    }
}
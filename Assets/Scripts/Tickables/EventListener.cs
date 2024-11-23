using Com.UnBocal.Rush.Properties;
using UnityEngine;
using UnityEngine.Events;

namespace Com.UnBocal.Rush.Tickables
{
    public class EventListener : MonoBehaviour
    {
        // Event
        [HideInInspector] public UnityEvent LocalTick = new UnityEvent();

        // Getters
        public bool IsTicking { get => _isTicking; }
        public bool IsListening { get => _isListening; }
        public int TickLeft { get => _tickLeft; }

        // Tick
        private bool _isTicking = false;
        private bool _isListening = true;
        private int _tickLeft = 0;

        // Running
        [SerializeField] private bool _destroyOnStopRunning = false;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Unity
        private void Start() => ConnectEvents();

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Initialization
        private void ConnectEvents()
        {
            Game.Events.Tick.AddListener(Tick);
            Game.Events.StopRunning.AddListener(OnStopRunning);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tick
        #region Tick

        private void Tick()
        {
            if (!CanTick()) return;
            LocalTick.Invoke();
        }

        private bool CanTick()
        {
            if (--_tickLeft > 0) return _isTicking = false;
            return _isTicking = true;
        }

        public void WaitFor(int pTickCount)
        {
            _tickLeft = pTickCount;
            if (_tickLeft < 1) return;
            _isTicking = false;
        }

        public void WaitFor() => WaitFor(1);

        public void ResetWaitTick() => WaitFor(0);

        public void SetTicking(bool pIsListening = true) => _isListening = pIsListening;
        #endregion

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Running
        #region Running
        private void OnStopRunning()
        {
            TryDestroy();
        }

        private void TryDestroy()
        {
            if (!_destroyOnStopRunning) return;
            Destroy(gameObject);
        }
        #endregion
    }
}

using Com.UnBocal.Rush.Properties;
using UnityEngine;
using UnityEngine.Events;

namespace Com.UnBocal.Rush.Tickables
{
    public class TickListener : MonoBehaviour
    {
        // Event
        public UnityEvent LocalTick = new UnityEvent();

        // Getters
        public bool IsListening { get => _isListening; }
        public int TickLeft { get => _tickLeft; }

        // Tick
        private bool _isListening = true;
        private int _tickLeft = 0;

        private void Start() => SetTickEvents();

        private void SetTickEvents() => Game.Signals.Tick.AddListener(Tick);

        private void Tick()
        {
            if (!CanTick()) return;
            LocalTick.Invoke();
        }

        private bool CanTick()
        {
            if (!_isListening) return false;
            if (--_tickLeft > 0) return false;
            return true;
        }

        public void WaitFor(int pTickCount) => _tickLeft = pTickCount;

        public void WaitFor() => WaitFor(1);

        public void SetTicking(bool pIsListening = true) => _isListening = pIsListening;
    }
}

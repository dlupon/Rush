using Com.UnBocal.Rush.Properties;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Com.UnBocal.Rush.Managers
{
    public class TickManager : MonoBehaviour
    {
        // Ticks
        [SerializeField] private float _tickwaitTime = 1f;
        private int _tickCount = 0;
        private bool _isTicking = false;

        private void Awake()
        {
            StartTicking();
        }

        private void StartTicking()
        {
            _isTicking = true;
            StartCoroutine(nameof(Ticking));
        }

        private IEnumerator Ticking()
        {
            yield return Tick(Game.Signals.FirstTick);
            while (_isTicking) yield return Tick(Game.Signals.Tick);
        }

        private IEnumerator Tick(UnityEvent pTickEvent)
        {
            UpdateGameSpeed();
            pTickEvent.Invoke();
            yield return new WaitForSeconds(_tickwaitTime);
        }

        private void UpdateGameSpeed()
        {
            Game.Properties.Speed = _tickwaitTime;
            Time.timeScale = 1f / _tickwaitTime;
        }
    }
}
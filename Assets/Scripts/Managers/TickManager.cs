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
        [SerializeField] private int _tickPerSecondTarget = TICK_PER_SECOND;
        private float _tickPerSecond = default;
        private const int TICK_PER_SECOND = 4;
        private float _tickIntervalTarget = default;
        private float _tickInterval = default;
        private int _tickCount = default;
        private bool _isTicking = false;

        private void Start() => StartTicking();

        private void StartTicking()
        {
            _isTicking = true;
            UpdateGameSpeed();
            StartCoroutine(nameof(Ticking));
        }

        private IEnumerator Ticking()
        {
            while (_isTicking) yield return Tick(Game.Signals.Tick);
        }

        private IEnumerator Tick(UnityEvent pTickEvent)
        {
            _tickCount++;
            UpdateGameSpeed();
            pTickEvent.Invoke();
            yield return new WaitForSeconds(_tickInterval);
        }

        private void UpdateGameSpeed()
        {
            _tickIntervalTarget = 1f / ((float)_tickPerSecondTarget); // One Second Divide By The Number Of Tick

            _tickInterval += (_tickIntervalTarget - _tickInterval) * .1f;
            _tickPerSecond += (_tickPerSecondTarget - _tickPerSecond) * .1f;

            Game.Properties.TickInterval = _tickInterval;
            Time.timeScale = _tickPerSecond / TICK_PER_SECOND; // Ratio Scaling The Time Based On The Number Of Tick Per Second
        }
    }
}
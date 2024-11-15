using Com.UnBocal.Rush.Properties;
using System.Collections;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

namespace Com.UnBocal.Rush.Managers
{
    public class TickManager : MonoBehaviour
    {
        // Ticks
        [SerializeField] private float _tickPerSecond = 1;
        private int _tickCount = default;

        private const float TICK_DURATION = 1f;
        private float _tickRatio = 0f;
        private float _time = 0f;

        private void Start() => StartTicking();

        private void Update()
        {
            Ticking();
        }

        private void StartTicking() => ResetTick();

        private void ResetTick()
        {
            _tickRatio = 0;
            _time = 0;
        }

        private void UpdateTimeTick()
        {
            _time += _tickPerSecond * Time.deltaTime;
            Game.Properties.SetTickRatio(_tickRatio = _time / TICK_DURATION);
        }

        private void Ticking()
        {
            if (_time >= TICK_DURATION)
            {
                ResetTick();
                Tick();
            }
            UpdateTimeTick();
        }
       

        private void Tick()
        {
            _tickCount++;
            Game.Signals.Tick.Invoke();
        }
    }
}
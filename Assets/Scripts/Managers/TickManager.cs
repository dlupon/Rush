using Com.UnBocal.Rush.Properties;
using UnityEngine;

namespace Com.UnBocal.Rush.Managers
{
    public class TickManager : MonoBehaviour
    {
        // Ticks
        [SerializeField, Range(.1f, 15f)] private float _tickPerSecond = 3;
        [SerializeField, Range(1, 5f)] private float _tickAdditionalSpeed = 1;
        private float _tickDefaultSpeed = 1f;
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
            if (_time < 1) _time += _tickPerSecond * (_tickDefaultSpeed + _tickAdditionalSpeed * Game.Properties.SliderValue) * Time.deltaTime;
            else _time = 1f;
            Game.Properties.SetTickRatio(_tickRatio = _time / TICK_DURATION);
        }

        private void Ticking()
        {
            if (_time >= TICK_DURATION && Game.Inputs.DEBUGNextTick())
            {
                ResetTick();
                Tick();
            }
            UpdateTimeTick();
        }
       

        private void Tick()
        {
            _tickCount++;
            Game.Events.Tick.Invoke();
        }
    }
}
using Com.UnBocal.Rush.Properties;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Com.UnBocal.Rush.Tickables
{
    public class Tickable : MonoBehaviour
    {
        // Components
        protected Transform _transform;

        // Tick
        protected int m_tickLeft = 0;

        private void Awake()
        {
            SetComponents();
            SetTickEvents();
            OnAwake();
        }

        protected virtual void SetComponents() { _transform = transform; }
        private void SetTickEvents() { Game.Signals.FirstTick.AddListener(FirstTick); Game.Signals.Tick.AddListener(Tick); }
        protected virtual void OnAwake() { }
        protected virtual void FirstTick() { }
        protected virtual void Tick() { }
    }
}

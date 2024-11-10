using UnityEngine;
using UnityEngine.Events;

namespace Com.UnBocal.Rush.Properties
{
    public class Game
    {
        public class Signals
        {
            public static UnityEvent FirstTick = new UnityEvent();
            public static UnityEvent Tick = new UnityEvent();
            public static UnityEvent DemiTick = new UnityEvent();
        }

        public class Properties
        {
            public static float TickInterval = 1f;
        }
    }
}
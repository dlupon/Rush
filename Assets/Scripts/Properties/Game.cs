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
            public static Grid WorldGrid { get => _worldGrid; }
            private static Grid _worldGrid = new Grid();
            public static float TickInterval = 1f;

            public static void SetGrid(Grid pGrid) => _worldGrid = pGrid;
        }
    }
}
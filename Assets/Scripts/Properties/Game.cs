using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Com.UnBocal.Rush.Properties
{
    public static class Game
    {
        public static class Events
        {
            // Game
            public static UnityEvent Running = new UnityEvent();
            public static UnityEvent StopRunning = new UnityEvent();

            // Tick
            public static UnityEvent FirstTick = new UnityEvent();
            public static UnityEvent Tick = new UnityEvent();
            public static UnityEvent DemiTick = new UnityEvent();
            
            // Level
            public static UnityEvent<Properties.ActionTile[]> ActionTilesUpdated = new UnityEvent<Properties.ActionTile[]>();   

            // Hud
            public static UnityEvent<Properties.ActionTile> TileSelected = new UnityEvent<Properties.ActionTile>();
        }

        public static class Properties
        {
            #region Variables
            // Getters
            //  // Game
            public static bool Running => _running;
            //  // Screen
            public static Vector2Int ScreenSizeWithPixelization => GetScreenSizeWithPixelization();
            public static Vector2Int ScreenSize => GetScreenSize();
            public static float ScreenPixelizedRatio => ScreenSizeWithPixelization.magnitude / ScreenSize.magnitude;
            //  // Tick
            public static float TickRatio => _tickRatio;

            // Game
            private static bool _running = false;

            // Screen Pixelization
            private static Vector2Int _screenSize = Vector2Int.zero;
            private static Vector2Int _screenSizeWithPixelization = Vector2Int.zero;
            private static float _screenPizelizationRatio = .3f;

            // Tick
            private static float _tickRatio;

            // Orientation
            public const float ROTATION = 90f;
            public enum Orientation { NORTH, EAST, SOUTH, WEST }

            // Collision
            private const string LAYER_CUBE = "Cube";
            private const string LAYER_TILE = "Tile";
            private const string LAYER_IGNORE = "Ignore";
            public static LayerMask LayerCube => LayerMask.NameToLayer(LAYER_CUBE);
            public static LayerMask LayerTile => LayerMask.NameToLayer(LAYER_TILE);
            public static LayerMask LayerIgnore => LayerMask.NameToLayer(LAYER_IGNORE);

            // Tiles
            public enum CubeType { WHITE, BLUE, GREEN, RED, ORANGE, PINK }
            public static ActionTile[] CurrentActionTiles => _currentActionTiles;
            private static ActionTile[] _currentActionTiles;
            [Serializable] public struct ActionTile
            {
                public GameObject Factory;
                public Orientation Direction;
                public int Count;
            }
            #endregion

            // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Screen
            #region Screen
            public static Vector2Int SetScreenPixelization(float pPizelizationRatio)
            {
                GetScreenSize();
                _screenPizelizationRatio = Mathf.Clamp01(pPizelizationRatio);
                _screenSizeWithPixelization.x = (int)(_screenSize.x * _screenPizelizationRatio);
                _screenSizeWithPixelization.y = (int)(_screenSize.y * _screenPizelizationRatio);
                return _screenSizeWithPixelization;
            }

            private static Vector2Int GetScreenSize()
            {
                if (_screenSize.x == Screen.width && _screenSize.y == Screen.height) return _screenSize;

                _screenSize.x = Screen.width;
                _screenSize.y = Screen.height;

                return _screenSize;
            }

            private static Vector2Int GetScreenSizeWithPixelization() => SetScreenPixelization(_screenPizelizationRatio);
            #endregion

            // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tick
            #region tick
            public static float SetTickRatio(float pTickRatio) => _tickRatio = pTickRatio;
            #endregion

            // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Game
            #region Game
            public static bool ToggleRunning() => SetRunning(!_running);
            public static bool SetRunning(bool pRunning)
            {
                _running = pRunning;
                if (_running) Events.Running.Invoke();
                else Events.StopRunning.Invoke();
                return _running;
            }
            #endregion

            // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Level
            #region Level
            public static void SetCurrentActionTiles(ActionTile[] pCurrentActionTiles)
            {
                _currentActionTiles = pCurrentActionTiles;
                Events.ActionTilesUpdated.Invoke(_currentActionTiles);
            }
            #endregion

        }

        public static class Inputs
        {
            // Debug
            private static bool _debug = false;

            // Inputs
            private const KeyCode RUN = KeyCode.Space;
            private const KeyCode DEBUG = KeyCode.LeftControl;
            private const KeyCode ALT = KeyCode.LeftAlt;

            // Keyboard
            public static bool Run => Input.GetKeyDown(RUN) || Input.GetMouseButtonDown((int)MouseButton.Middle);

            // Mouse
            public static Vector3 MousePosition => GetMousePosition();

            private static Vector3 _mousePosition = Vector3.zero;

            // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Keyboard

            public static bool DEBUGNextTick()
            {
                if (Input.GetKeyDown(DEBUG)) _debug = !_debug;

                if (_debug)
                {
                    if (Input.GetKeyDown(ALT)) return true;
                    return false;
                }

                return true;
            }

            // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Mouse
            public static Vector3 GetMousePosition()
            {
                _mousePosition = Input.mousePosition.normalized; 
                return _mousePosition *= Input.mousePosition.magnitude * (Properties.ScreenPixelizedRatio);
            }
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Custom Methods
        #region Custom Methods
        public static Vector3 Round(this Vector3 pVector)
        {
            pVector.x = Mathf.Round(pVector.x);
            pVector.y = Mathf.Round(pVector.y);
            pVector.z = Mathf.Round(pVector.z);
            return pVector;
        }

        public static Vector2Int ToInt(this Vector2 pVector)
        {
            Vector2Int ptest = Vector2Int.zero;
            ptest.x = (int)pVector.x;
            ptest.y = (int)pVector.y;
            return ptest;
        }
        #endregion

    }
}
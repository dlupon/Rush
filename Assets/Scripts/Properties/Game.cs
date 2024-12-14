using System;
using System.Collections.Generic;
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
            public static UnityEvent ToggleRuning = new UnityEvent();
            public static UnityEvent Running = new UnityEvent();
            public static UnityEvent StopRunning = new UnityEvent();
            public static UnityEvent End = new UnityEvent();
            public static UnityEvent<Properties.ActionTile[]> ActionTilesUpdated = new UnityEvent<Properties.ActionTile[]>();

            // Tick
            public static UnityEvent Tick = new UnityEvent();
            
            // Level
            public static UnityEvent LaunchLevel = new UnityEvent();
            public static UnityEvent<int> LevelLoad = new UnityEvent<int>();
            public static UnityEvent<Transform> LevelLoaded = new UnityEvent<Transform>();

            // Main Menu
            public static UnityEvent LaunchGame = new UnityEvent();

            // Hud / Tile Placer
            public static UnityEvent<Properties.ActionTile> TileSelected = new UnityEvent<Properties.ActionTile>();
            public static UnityEvent<Transform> TilePlaced = new UnityEvent<Transform>();
            public static UnityEvent<Vector3> LevelTouched = new UnityEvent<Vector3>();
        }

        public static class Properties
        {
            #region Variables
            // Getters
            //  // Screen
            public static Vector2Int ScreenSizeWithPixelization => GetScreenSizeWithPixelization();
            public static Vector2Int ScreenSize => GetScreenSize();
            public static float ScreenPixelizedRatio => ScreenSizeWithPixelization.magnitude / ScreenSize.magnitude;
            //  // Tick
            public static float SpeedMin => .25f;
            public static float SliderValue = 0f;
            public static float TickRatio => _tickRatio;

            // Game
            public static bool InGame = false;
            public static Transform CurrentLevel => _currentLevel;
            private static Transform _currentLevel;
            public static int CubeCount => _cubeCount;
            private static int _cubeCount;
            public static bool Running => _running;
            private static bool _running = false;

            // Level
            public static List<GameObject> Levels => _levels;
            private static List<GameObject> _levels;


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
            private const string LAYER_ACTIONTILE = "ActionTile";
            private const string LAYER_IGNORE = "Ignore";
            public static LayerMask LayerCube => LayerMask.NameToLayer(LAYER_CUBE);
            public static LayerMask LayerTile => LayerMask.NameToLayer(LAYER_TILE);
            public static LayerMask LayerActionTile => LayerMask.NameToLayer(LAYER_ACTIONTILE);
            public static LayerMask LayerIgnore => LayerMask.NameToLayer(LAYER_IGNORE);

            // Tiles
            public enum CubeType { WHITE, BLUE, GREEN, RED, ORANGE, PINK }
            public static ActionTile[] CurrentActionTiles => _currentActionTiles;
            private static ActionTile[] _currentActionTiles;
            [Serializable] public class ActionTile
            {
                public int MaxCount => _maxCount;
                public bool CanPlaceNewTile => Tiles.Count < _maxCount;
                public List<Transform> Tiles => CreateTilesist();

                [SerializeField] private int _maxCount;
                public GameObject Factory;
                public Orientation Direction;

                private List<Transform> _tiles;

                private List<Transform> CreateTilesist()
                {
                    if (_tiles != null) return _tiles;
                    return _tiles = new List<Transform>();
                }

                public void AddTile(Transform pTile)
                {
                    if (ContainsTile(pTile)) return;
                    _tiles.Add(pTile);
                }

                public void RemoveTile(GameObject pTile) => RemoveTile(pTile.transform);

                public void RemoveTile(Transform pTile)
                {
                    if (!ContainsTile(pTile)) return;
                    _tiles.Remove(pTile);
                }

                public bool ContainsTile(Transform _tile) => _tiles.Contains(_tile);
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

            public static void SetLevel(Transform pLevel)
            {
                _currentLevel = pLevel;

                Debug.Log($"{_currentLevel.name} loaded");

                Events.LevelLoaded.Invoke(_currentLevel);
            }

            public static void CubeSpawn() => _cubeCount++;

            public static void CubeDies()
            {
                --_cubeCount;
                if (_cubeCount > 0) return;
                Events.End.Invoke();
            }
            #endregion

            // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Level
            #region Level
            public static void SetLevels(List<GameObject> pLevels) => _levels = pLevels;

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

        public static Vector3 Multiply(this Vector3 pVector, float pX, float pY, float pZ) => pVector.Multiply(new Vector3(pX, pY, pZ));

        public static Vector3 Multiply(this Vector3 pVector, Vector3 pMultiplyer)
        {
            pVector.x *= pMultiplyer.x;
            pVector.y *= pMultiplyer.y;
            pVector.z *= pMultiplyer.z;
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
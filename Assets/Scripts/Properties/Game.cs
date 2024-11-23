using UnityEngine;
using UnityEngine.Events;

namespace Com.UnBocal.Rush.Properties
{
    public static class Game
    {
        public static class Signals
        {
            public static UnityEvent FirstTick = new UnityEvent();
            public static UnityEvent Tick = new UnityEvent();
            public static UnityEvent DemiTick = new UnityEvent();
        }

        public static class Properties
        {
            // Getters
            //  // Screen
            public static Vector2Int ScreenSizeWithPixelization => GetScreenSizeWithPixelization();
            public static Vector2Int ScreenSize => GetScreenSize();
            public static float ScreenPixelizedRatio => ScreenSizeWithPixelization.magnitude / ScreenSize.magnitude;
            //  // Tick
            public static float TickRatio => _tickRatio;

            // Screen Pixelization
            private static Vector2Int _screenSize = Vector2Int.zero;
            private static Vector2Int _screenSizeWithPixelization = Vector2Int.zero;
            private static float _screenPizelizationRatio = .3f;

            // Tick
            private static float _tickRatio;

            // ----------------~~~~~~~~~~~~~~~~~~~==========================# // Tick
            #region tick
            public static float SetTickRatio(float pTickRatio) => _tickRatio = pTickRatio;
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
        
        }

        public static class Inputs
        {
            public static Vector3 MousePosition => GetMousePosition();

            private static Vector3 _mousePosition = Vector3.zero;

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
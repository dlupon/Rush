using UnityEngine;

namespace Com.UnBocal.Rush.Managers
{
    public static class InputManager
    {
        public enum MouseButton { Left, Right, Mid }

        public static bool LeftClick => ClickOrHold(MouseButton.Left);
        public static bool LeftHold => !ClickOrHold(MouseButton.Left);

        private static float _holdDurationMin = .5f;
        public static float _waitTime = 0f;

        private static bool IsClick(MouseButton _mouseButton)
        {
            if (!IsPressing(_mouseButton)) return false;
            
            bool _isClick = ClickOrHold(_mouseButton);
            
            return _isClick;
        }

        private static bool ClickOrHold(MouseButton _mouseButton)
        {
            _waitTime += Time.deltaTime;
            if (Input.GetMouseButtonUp((int)_mouseButton) && _waitTime < _holdDurationMin) return true;
            return false;
        }

        private static bool IsPressing(MouseButton _mouseButton)
        {
            if (Input.GetMouseButton((int)_mouseButton) || Input.GetMouseButtonUp((int)_mouseButton)) return true;
            _waitTime = 0f;
            return false;
        }
    }
}
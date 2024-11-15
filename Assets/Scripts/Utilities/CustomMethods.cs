using UnityEngine;

namespace Com.UnBocal.Rush.Utilities
{
    public static class CustomMethods
    {
        public static Vector3 Round(this Vector3 pVector)
        {
            pVector.x = Mathf.Round(pVector.x);
            pVector.y = Mathf.Round(pVector.y);
            pVector.z = Mathf.Round(pVector.z);
            return pVector;
        }
    }
}
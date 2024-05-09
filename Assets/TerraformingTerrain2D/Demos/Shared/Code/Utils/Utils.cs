using UnityEngine;

namespace DemosShared
{
    public static class Utils
    {
        public static T[] FindObjects<T>() where T : MonoBehaviour
        {
            #if UNITY_2023
            return Object.FindObjectsByType<T>(FindObjectsSortMode.None);
            #else
            return Object.FindObjectsOfType<T>();
            #endif
        }
    }
}
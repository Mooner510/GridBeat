using UnityEngine;

namespace Utils {
    public static class GameObjectUtils {
        public delegate void SetUp<T>(ref T obj);
        
        public static void SetPosition(this Transform transform, SetUp<Vector3> setUp) {
            var pos = transform.position;
            setUp.Invoke(ref pos);
            transform.position = pos;
        }
        
        public static void SetLocalPosition(this Transform transform, SetUp<Vector3> setUp) {
            var pos = transform.localPosition;
            Debug.Log($"before: pos{pos}");
            setUp.Invoke(ref pos);
            Debug.Log($"before: pos{pos}");
            transform.localPosition = pos;
        }
        
        public static void SetLocalScale(this Transform transform, SetUp<Vector3> setUp) {
            var pos = transform.localScale;
            setUp.Invoke(ref pos);
            transform.localScale = pos;
        }
    }
}
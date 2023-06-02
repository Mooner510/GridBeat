using UnityEngine;
using Utils;

public class SingleMono<T> : MonoBehaviour where T : MonoBehaviour {
    private static T _init;
    private static readonly object Sync = new object();

    public static T Instance {
        get {
            lock (Sync) {
                if (_init is not null) return _init;
                var objects = FindObjectsOfType<T>();
                if (objects.Length > 0) _init = objects[0];
                else if (objects.Length > 1)
                    Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");

                if (_init is not null) return _init;
                var objectName = typeof(T).ToString();
                var gameObject = GameObject.Find(objectName);
                if (gameObject.IsDestroyed()) gameObject = new GameObject(objectName);
                _init = gameObject.AddComponent<T>();
                return _init;
            }
        }
    }

    // protected void Awake() {
    //     if (_init == null) _init = this as T;
    //     else if (_init != this) Destroy(gameObject);
    //     DontDestroyOnLoad(gameObject);
    // }
}

public class SingleTon<T> where T : new() {
    private static T _init;

    public static T Instance {
        get {
            if (_init is not null) return _init;
            return _init = new T();
        }
    }
}
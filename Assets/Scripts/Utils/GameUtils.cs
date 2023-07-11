using Musics.Data;
using UnityEngine;

namespace Utils {
    public static class GameUtils {
        public static readonly Color ClearWhite = new Color(1, 1, 1, 0);
    
        public static Vector2 Locator(GameMode gameMode, int n) => gameMode == GameMode.Keypad ?
            new Vector2(-3 + 3 * (n % 3), 3 - 3 * (n / 3)) :
            // ReSharper disable once PossibleLossOfFraction
            new Vector2(-1.25f + 2.5f * (n % 2), 1.25f - 2.5f * (n / 2));

        private static RectTransform _canvas;

        public static RectTransform Canvas => _canvas == null ? _canvas = GameObject.Find("Canvas").GetComponent<RectTransform>() : _canvas;

        public static Vector2 LocationToCanvas(Vector2 vector) {
            if (Camera.main is null) return new Vector2();
            Vector2 viewPosition = Camera.main.WorldToViewportPoint(vector);
            var delta = Canvas.sizeDelta;
            return new Vector2(viewPosition.x * delta.x - delta.x / 2, viewPosition.y * delta.y - delta.y / 2);
        }
    
        public static Vector2 TransferCanvasLocation(Vector2 vector) => new(vector.x + 366, vector.y + 305.8f);
        
        public static bool IsDestroyed(this GameObject gameObject) => gameObject == null && !ReferenceEquals(gameObject, null);
    }
}
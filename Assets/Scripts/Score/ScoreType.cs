using System;
using UnityEngine;

namespace Score {
    public enum ScoreType {
        Perfect,
        Great,
        Good,
        Bad,
        Miss
    }

    public static class ScoreExtensions {
        public static string GetTag(this ScoreType score) {
            return score switch {
                ScoreType.Perfect => "PERFECT!",
                ScoreType.Great => "GREAT",
                ScoreType.Good => "GOOD",
                ScoreType.Bad => "BAD",
                ScoreType.Miss => "MISS",
                _ => throw new ArgumentOutOfRangeException(nameof(score), score, null)
            };
        }

        private static readonly Color Perfect = new(0.6f, 1f, 1f, 1f);
        private static readonly Color Great = new(0.4f, 1f, 0.4f, 1f);
        private static readonly Color Good = new(0.3f, 0.6f, 0.9f, 1f);
        private static readonly Color Bad = new(1f, 0.27f, 0.27f, 1f);
        private static readonly Color Miss = new(0.6f, 0.6f, 0.6f, 1f);
    
        public static Color GetColor(this ScoreType score) {
            return score switch {
                ScoreType.Perfect => Perfect,
                ScoreType.Great => Great,
                ScoreType.Good => Good,
                ScoreType.Bad => Bad,
                ScoreType.Miss => Miss,
                _ => throw new ArgumentOutOfRangeException(nameof(score), score, null)
            };
        }
    
        public static int GetBaseScore(this ScoreType score) {
            return score switch {
                ScoreType.Perfect => 300,
                ScoreType.Great => 200,
                ScoreType.Good => 100,
                ScoreType.Bad => 50,
                ScoreType.Miss => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(score), score, null)
            };
        }
    }
}
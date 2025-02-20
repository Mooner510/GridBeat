using _scenes.InGame.Player;
using Score;
using UnityEngine;
using Utils;

namespace Musics.Data {
    public static class NewNoteManager {
        private static PlayableNote[] _noteData;
        private static int _index;
        private static int _noteSpeed = 80;
        private static float _noteTime = 1;
        private static float _allowedTime = 0.3f;
        private static int _inputDelay;

        public static float GetNoteSpeed() => _noteSpeed / 10f;
        public static float GetNoteTime() => _noteTime * (NewMusicManager.GetGameMode() == GameMode.Keypad ? 1 : 2);
        public static float GetNoteAllowedTime() => _allowedTime;
        public static int GetInputDelay() => _inputDelay;
        public static void SetInputDelay(int value) => _inputDelay = value;
        
        // public static ScoreType GetPerfect(float diff) {
        //     diff -= _inputDelay;
        //     if (diff <= _allowedTime * 1000 / 4f) return ScoreType.Perfect;
        //     if (diff <= _allowedTime * 1000 / 3f) return ScoreType.Great;
        //     if (diff <= _allowedTime * 1000 / 1.65f) return ScoreType.Good;
        //     return diff <= _allowedTime * 1000 ? ScoreType.Bad : ScoreType.Miss;
        // }
        
        public static ScoreType GetPerfect(float diff) {
            diff -= _inputDelay;
            if (diff <= _allowedTime * 1000 / 3f) return ScoreType.Perfect;
            if (diff <= _allowedTime * 1000 / 2f) return ScoreType.Great;
            if (diff <= _allowedTime * 1000 / 1.2f) return ScoreType.Good;
            return diff <= _allowedTime * 1000 ? ScoreType.Bad : ScoreType.Miss;
        }

        private static void FixTime() {
            _noteSpeed = _noteSpeed.Between(160, 10);
            var subTime = _noteSpeed / 10;
            _noteTime = subTime switch {
                < 8 => 2.7144f - subTime * 0.2143f,
                > 8 => 1.5f - subTime * 0.0625f,
                _ => 1
            };
            _allowedTime = 0.3f;
        }

        public static void NoteSpeedUp(bool shift) {
            _noteSpeed += shift ? 10 : 1;
            FixTime();
        }

        public static void NoteSpeedDown(bool shift) {
            _noteSpeed -= shift ? 10 : 1;
            FixTime();
        }

        public static bool IsTop(int i) => _index + i >= _noteData.Length;

        public static PlayableNote Pick() => _noteData[_index];

        public static PlayableNote Pick(int i) => _noteData[_index + i];

        public static PlayableNote Pop() => _noteData[_index++];

        public static void LoadCurrentData() =>
            _noteData = NewMusicManager.Instance.GetMusicData().CreatePlayableNote();

        public static PlayableNote[] GetNoteData() => _noteData;

        public static void Start(float after) {
            _index = 0;
            LoadCurrentData();
            Debug.Log($"Loaded Note Data: {_noteData.Length}");
            Debug.Log("Data Start");
            Ticker.Instance.Write(after);
        }

        public static void Stop() {
            Debug.Log("Stop and Save");
            Ticker.Instance.StopWriteSoftness();
        }
    }
}
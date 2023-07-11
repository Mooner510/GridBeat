using System.Collections.Generic;
using System.IO;
using System.Linq;
using Musics.Data;
using UnityEngine;
using UnityEngine.Video;
using Utils;

namespace Musics {
    public class NewMusicManager : SingleTon<NewMusicManager> {
        private List<NewMusicData> _quadMusic;
        private List<NewMusicData> _keypadMusic;
        private int _quadSelection;
        private int _keypadSelection;
        private static GameMode _gameMode = GameMode.Keypad;
        private static MusicDifficulty _difficulty = MusicDifficulty.Easy;

        public static void SetGameMode(GameMode gameMode) => _gameMode = gameMode;

        public static GameMode GetGameMode() => _gameMode;

        public static void DifficultyUp() => _difficulty++;

        public static void DifficultyDown() => _difficulty--;

        public static MusicDifficulty GetDifficulty() => _difficulty;
        // if (SceneManager.GetActiveScene().name.Equals("End")) return _gameMode;   
        // return _gameMode = SceneManager.GetActiveScene().name.Equals("InGame") ? GameMode.Keypad : GameMode.Quad;

        public int GetMusicId() => _gameMode == GameMode.Keypad ? _keypadSelection : _quadSelection;

        private int GetMusics() => _gameMode == GameMode.Keypad ? _keypadMusic.Count : _quadMusic.Count;

        public NewMusicData GetMusicData() =>
            _gameMode == GameMode.Keypad ? _keypadMusic[_keypadSelection % _keypadMusic.Count] : _quadMusic[_quadSelection % _quadMusic.Count];

        public NewMusicData GetMusicData(int addition) =>
            _gameMode == GameMode.Keypad
                ? _keypadMusic[(_keypadSelection + addition) % _keypadMusic.Count]
                : _quadMusic[(_quadSelection + addition) % _quadMusic.Count];

        public NewMusicData GetMusicDataAdd(int addition) =>
            _gameMode == GameMode.Keypad
                ? _keypadMusic[(_keypadSelection += addition) % _keypadMusic.Count]
                : _quadMusic[(_quadSelection += addition) % _quadMusic.Count];

        public NewMusicData GetMusicDataSet(int set) =>
            _gameMode == GameMode.Keypad ? _keypadMusic[(_keypadSelection = set) % _keypadMusic.Count] : _quadMusic[(_quadSelection = set) % _quadMusic.Count];

        // public void UpdateCurrentMusicData() => _musicDataList[_selection] = _musics.musics[_selection].ToMusicData();

        public NewMusicManager() => ReloadAll();

        public NewMusicData Next() {
            var data = GetMusicDataAdd(1);
            if (data.mapData.ContainsKey(_difficulty)) return data;
            var list = data.mapData.Keys.ToList();
            list.Sort();
            _difficulty = list[0];
            return data;
        }

        public NewMusicData Back() {
            var data = GetMusicDataAdd(-1);
            if (data.mapData.ContainsKey(_difficulty)) return data;
            var list = data.mapData.Keys.ToList();
            list.Sort();
            _difficulty = list[0];
            return data;
        }

        private void ReloadAll() {
            Debug.Log("Reloading All Musics");

            List<NewMusicData> quadMusic = new List<NewMusicData>();
            List<NewMusicData> keypadMusic = new List<NewMusicData>();
            var directories = Directory.GetDirectories("Assets/Data/Maps");
            foreach (var directory in directories) {
                var info = Json.LoadJsonFile<NewMusicInfo>($"Assets/Data/Maps/{directory}/data.json");

                var mapDic = new Dictionary<MusicDifficulty, MapData>();
                foreach (var difficulty in MusicDifficultyUtils.MusicDifficulties) {
                    try {
                        mapDic.Add(difficulty,
                            Json.LoadJsonFile<MapData>($"Assets/Data/Maps/{directory}/map-{difficulty.ToString()}.json"));
                    } catch {
                        // ignored
                    }
                }

                (info.isKeypad ? keypadMusic : quadMusic).Add(new NewMusicData {
                    musicInfo = info,
                    mapData = mapDic,
                    audio = Resources.Load<AudioClip>($"Assets/Data/Maps/{directory}/audio.ogg"),
                    image = Resources.Load<Sprite>($"Assets/Data/Maps/{directory}/image.png"),
                    blurImage = Resources.Load<Sprite>($"Assets/Data/Maps/{directory}/blur.png"),
                    previewAudio = Resources.Load<AudioClip>($"Assets/Data/Maps/{directory}/preview.MP3"),
                    video = Resources.Load<VideoClip>($"Assets/Data/Maps/{directory}/video.mp4")
                });
            }

            _quadMusic = quadMusic;
            _keypadMusic = keypadMusic;
            Debug.Log("Reloaded All Musics Completely");
        }
    }
}
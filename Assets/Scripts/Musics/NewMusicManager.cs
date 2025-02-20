﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Musics.Data;
using UnityEngine;
using UnityEngine.Video;
using Utils;
using Object = System.Object;

namespace Musics {
    public class NewMusicManager : SingleTon<NewMusicManager> {
        private List<NewMusicData> _quadMusic;
        private List<NewMusicData> _keypadMusic;
        private int _quadSelection;
        private int _keypadSelection;
        private static GameMode _gameMode = GameMode.Quad;
        private static MusicDifficulty _difficulty = MusicDifficulty.Easy;

        public static void SetGameMode(GameMode gameMode) => _gameMode = gameMode;

        public static GameMode GetGameMode() => _gameMode;

        public static void DifficultyUp() {
            var maps = Instance.GetMusicData().mapData;
            do {
                _difficulty = _difficulty.Next();
            } while (!maps.ContainsKey(_difficulty));
        }

        public static void DifficultyDown() {
            var maps = Instance.GetMusicData().mapData;
            do {
                _difficulty = _difficulty.Prev();
            } while (!maps.ContainsKey(_difficulty));
        }

        public static MusicDifficulty GetDifficulty() => _difficulty;
        // if (SceneManager.GetActiveScene().name.Equals("End")) return _gameMode;   
        // return _gameMode = SceneManager.GetActiveScene().name.Equals("InGame") ? GameMode.Keypad : GameMode.Quad;

        public int GetMusicId() => _gameMode == GameMode.Keypad ? _keypadSelection : _quadSelection;

        private int GetMusics() => _gameMode == GameMode.Keypad ? _keypadMusic.Count : _quadMusic.Count;

        public NewMusicData GetMusicData() {
            var data = _gameMode == GameMode.Keypad ? _keypadMusic[_keypadSelection] : _quadMusic[_quadSelection];
            while (!data.mapData.ContainsKey(_difficulty)) _difficulty = _difficulty.Next();
            return data;
        }

        public NewMusicData GetMusicData(int addition) =>
            _gameMode == GameMode.Keypad
                ? _keypadMusic[(_keypadSelection + addition + _keypadMusic.Count * 3) % _keypadMusic.Count]
                : _quadMusic[(_quadSelection + addition + _quadMusic.Count * 3) % _quadMusic.Count];

        public NewMusicData GetMusicDataAdd(int addition) {
            if (_gameMode == GameMode.Keypad) _keypadSelection = (_keypadSelection + addition + _keypadMusic.Count) % _keypadMusic.Count;
            else _quadSelection = (_quadSelection + addition + _quadMusic.Count) % _quadMusic.Count;
            return GetMusicData();
        }

        public NewMusicData GetMusicDataSet(int set) {
            if (_gameMode == GameMode.Keypad) _keypadSelection = set % _keypadMusic.Count;
            else _quadSelection = set % _quadMusic.Count;
            return GetMusicData();
        }

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
            var directories = FileUtils.GetDirectories("Assets/Resources/Maps");
            if (directories.Length <= 0) directories = FileUtils.GetDirectories("BeatTable_Data/Resources/Maps");
            Debug.Log($"Founded Maps: {string.Join(", ", directories)}");
            foreach (var directory in directories) {
                var info = Json.LoadJsonFile<NewMusicInfo>($"{directory}/data.json");

                var mapDic = new Dictionary<MusicDifficulty, MapData>();
                foreach (var difficulty in MusicDifficultyUtils.MusicDifficulties) {
                    try {
                        mapDic.Add(difficulty,
                            Json.LoadJsonFile<MapData>($"{directory}/map-{difficulty.ToString()}.json"));
                        Debug.Log($"{directory} - Founded Level: {difficulty.ToString()}");
                    } catch {
                        // ignored
                    }
                }

                var newPath = directory[directory.IndexOf("Maps", StringComparison.Ordinal)..].Replace('\\', '/');
                Debug.Log($"DIR: {newPath}");
                var musicData = new NewMusicData {
                    musicInfo = info,
                    mapData = mapDic,
                    audio = Resources.Load<AudioClip>($"{newPath}/audio"),
                    background = Resources.Load<Sprite>($"{newPath}/background"),
                    image = Resources.Load<Sprite>($"{newPath}/image"),
                    blurImage = Resources.Load<Sprite>($"{newPath}/blur"),
                    previewAudio = Resources.Load<AudioClip>($"{newPath}/preview"),
                    video = Resources.Load<VideoClip>($"{newPath}/video")
                };
                (info.isKeypad ? keypadMusic : quadMusic).Add(musicData);
            }

            _quadMusic = quadMusic;
            _keypadMusic = keypadMusic;
            Debug.Log("Reloaded All Musics Completely");
        }

        private static T LoadResources<T>(string path) where T : UnityEngine.Object {
            var load = Resources.Load<T>(path);
            Debug.Log(load == null ? $"No Load: {path}" : $"Yes Load: {path}");
            return load;
        }
    }
}
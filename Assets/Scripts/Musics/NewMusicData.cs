using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Video;

namespace Musics {
    public enum MusicDifficulty {
        Easy, Normal, Hard, Master
    }

    public static class MusicDifficultyUtils {
        public static readonly MusicDifficulty[] MusicDifficulties =
            {MusicDifficulty.Easy, MusicDifficulty.Normal, MusicDifficulty.Hard, MusicDifficulty.Master};
    }
    
    [Serializable]
    public struct NewMusicData {
        public NewMusicInfo musicInfo;
        public Dictionary<MusicDifficulty, MapData> mapData;
        [CanBeNull] public VideoClip video;
        public Sprite image;
        public Sprite blurImage;
        public AudioClip audio;
        public AudioClip previewAudio;
    }
    
    [Serializable]
    public struct NewMusicInfo {
        public string name;
        public string artist;
        public int playTime;
        public bool isKeypad;
    }

    [Serializable]
    public struct MapData {
        public int level;
        public NewNote[] notes;
    }
    
    public static class MapDataUtils {
        public static PlayableNote[] CreatePlayableNote(this NewMusicData data) {
            var mapData = data.mapData[NewMusicManager.GetDifficulty()];
            
            var length = mapData.notes.Length;
            var notes = new PlayableNote[length];
            for (var i = 0; i < length; i++) {
                notes[i] = new PlayableNote {
                    note = mapData.notes[i],
                    isClicked = false
                };
            }

            return notes;
        }
    }

    [Serializable]
    public struct NewNote {
        public int key;
        public long offset;
    }

    [Serializable]
    public struct PlayableNote {
        public NewNote note;
        public bool isClicked;
    }
}
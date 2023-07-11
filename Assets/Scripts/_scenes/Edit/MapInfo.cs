using System;
using Musics.Data;

namespace _scenes.Edit {
    [Serializable]
    public class MapInfo {
        public MusicInfo musicInfo;
        public LevelInfo levelInfo;
        public BackgroundInfo backgroundInfo;
        public NoteMap noteMap;
    }

    [Serializable]
    public class MusicInfo {
        public string musicPath;
        public float bpm;
        public float volume;
        public float offset;
        public float pitch;
        public int playTime;
    }

    public enum HeatSound {
        Kick
    }

    [Serializable]
    public class LevelInfo {
        public string artist;
        public string title;
        public string maker;
        public HeatSound heatSound = HeatSound.Kick;
        public float heatSoundVolume;
    }

    [Serializable]
    public class BackgroundInfo {
        public string backgroundColor;
        public string backgroundImagePath;
        public string backgroundImageTint;
    }

    [Serializable]
    public class NoteMap {
        public NoteData[] quadMap;
        public NoteData[] keypadMap;
    }
}
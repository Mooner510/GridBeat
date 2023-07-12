using System;
using Musics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace _scenes.InGame {
    public class BackgroundShower : MonoBehaviour {
        [SerializeField] private Image image;
        [SerializeField] private VideoPlayer video;

        public static BackgroundShower shower;

        private void Start() {
            shower = this;
            var data = NewMusicManager.Instance.GetMusicData();
            if (data.video != null) {
                video.clip = data.video;
                return;
            }
            video.clip = null;

            if (data.background != null) {
                image.sprite = data.background;
                image.color = new Color(1, 1, 1, 0.3f);
            } else {
                image.color = Color.clear;
            }
        }

        public void Play() {
            if (video.clip != null) {
                video.Play();
            }
        }
    }
}
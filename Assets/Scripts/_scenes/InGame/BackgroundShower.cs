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
            image.color = Color.clear;
            video.clip = null;
            
            var data = NewMusicManager.Instance.GetMusicData();
            if (data.video != null) {
                Debug.Log("ADDED VIDEO");
                video.clip = data.video;
            }

            if (data.background == null) return;
            image.sprite = data.background;
            image.color = Color.white;
        }

        public void Play() {
            video.Stop();
            if (video.clip != null) {
                video.Play();
            }
        }
    }
}
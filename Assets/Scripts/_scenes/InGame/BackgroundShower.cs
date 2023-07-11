using System;
using Musics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace _scenes.InGame {
    public class BackgroundShower : MonoBehaviour {
        [SerializeField] private Image image;
        [SerializeField] private VideoPlayer video;

        private void Start() {
            var data = NewMusicManager.Instance.GetMusicData();
            if (data.video != null) {
                video.clip = data.video;
                video.Play();
                return;
            }

            if (data.background != null) {
                image.sprite = data.background;
                image.color = new Color(1, 1, 1, 0.3f);
            } else {
                image.color = Color.clear;
            }
        }
    }
}
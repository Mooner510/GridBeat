﻿using System.Collections;
using _scenes.InGame.Listener;
using _scenes.InGame.Map;
using DG.Tweening;
using Musics;
using Musics.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace _scenes.InGame.Player {
    public class Player : SingleMono<Player> {
        [SerializeField] protected GameObject beatInspector;
        [SerializeField] private SpriteRenderer hider;
        // [SerializeField] private Text recording;

        [SerializeField] protected Sprite[] arrows;

        private static readonly Color ClickColor = new(0f, 0.8f, 0.8f, 0.3f);

        private void Start() {
            StartCoroutine(Init());
        }

        public void Stop() {
            StopCoroutine(Init());
            StartCoroutine(End());
        }

        private IEnumerator End() {
            NewNoteManager.Stop();
            var color = hider.color;
            for (var i = 0f; i <= 3; i += Time.deltaTime) {
                color.a = i / 3;
                hider.color = color;
                yield return null;
            }
            hider.color = Color.black;
            SceneManager.LoadScene("End");
        }

        private IEnumerator Init() {
            var color = hider.color;
            Ticker.Instance.ResetWrite();
            NewNoteManager.Start(4);
            for (var i = 0f; i <= 3; i += Time.deltaTime) {
                color.a = 1 - i / 3;
                hider.color = color;
                yield return null;
            }
            hider.color = Color.clear;
            yield return new WaitForSecondsRealtime(1);
            var data = NewMusicManager.Instance.GetMusicData();
            yield return new WaitForSecondsRealtime(data.musicInfo.playTime);
            StartCoroutine(End());
        }

        protected static IEnumerator Follow(GameObject obj, int note) {
            while (true) {
                if (obj.IsDestroyed()) break;
                obj.transform.position = MapMaker.Instance.GetNote(note).transform.position;
                yield return null;
            }
        }

        public virtual IEnumerator Accept(PlayableNote note, float time) {
            // // 아에 안 멈출거 같은데?
            // if(time > 0) yield return new WaitForSecondsRealtime(time);
            var obj = Instantiate(beatInspector, GameUtils.Locator(GameMode.Keypad, note.note.key), Quaternion.identity);
            StartCoroutine(Follow(obj, note.note.key));
            
            obj.transform.DOScale(Vector3.one * 2f, time).SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(time - KeyListener.AllowedTime);
            KeyListener.Instance.Queue(note);
            yield return new WaitForSecondsRealtime(KeyListener.AllowedTime);
            obj.transform.DOScale(Vector3.one * 2.35f, KeyListener.AllowedTime).SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(KeyListener.AllowedTime);
            Destroy(obj);
        }
    }
}
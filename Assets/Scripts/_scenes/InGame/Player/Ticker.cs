using System;
using System.Collections;
using _scenes.InGame.Listener;
using Musics;
using Musics.Data;
using UnityEngine;

namespace _scenes.InGame.Player {
    public class Ticker : SingleMono<Ticker> {
        [SerializeField] private AudioSource beatSound;
        [SerializeField] private AudioSource musicSound;

        private float _writeTime;
        private float _focusOutTime;

        public void Beat() {
            // if (_beat) return;
            beatSound.PlayOneShot(beatSound.clip);
            // _beat = true;
        }

        public float GetStartTime() => _writeTime;

        private void OnApplicationFocus(bool hasFocus) {
            if (hasFocus) _writeTime += Time.realtimeSinceStartup - _focusOutTime;
            else _focusOutTime = Time.realtimeSinceStartup;
        }

        public float GetPlayTime() => (Time.realtimeSinceStartup - _writeTime) * 1000 - NewNoteManager.GetInputDelay();

        public void ResetWrite() => _writeTime = Time.realtimeSinceStartup;

        public void Write(float after) {
            Debug.Log("Write Start");
            _writeTime = Time.realtimeSinceStartup + after;
            StartCoroutine(PlayIt(after));
        }

        private IEnumerator PlayIt(float after) {
            yield return new WaitForSecondsRealtime(after);
            // _routine = StartCoroutine(ReadTick());
            musicSound.PlayOneShot(NewMusicManager.Instance.GetMusicData().audio);
            BackgroundShower.shower.Play();
        }

        public void StopWrite() {
            Debug.Log("Write Stop");
            // if (_routine == null) return;
            // StopCoroutine(_routine);
            musicSound.Stop();
        }

        public void StopWriteSoftness() {
            Debug.Log("Write Stop");
            // if (_routine == null) return;
            // StopCoroutine(_routine);
            StartCoroutine(StopMusic());
        }

        private IEnumerator StopMusic() {
            for (var i = 0f; i <= 2; i += Time.deltaTime) {
                yield return null;
                musicSound.volume = 1 - 0.5f * i;
            }
            musicSound.Stop();
            musicSound.volume = 1;
        }

        private void Update() {
            if (NewNoteManager.IsTop(0)) return;
            Tick();
        }

        protected virtual void Tick() {
            var now = GetPlayTime() + KeyListener.NoteTime;
            var i = 0;
            do {
                var note = NewNoteManager.Pick(i).note;
                if (note.offset <= now) {
                    StartCoroutine(Player.Instance.Accept(NewNoteManager.Pop(), now - note.offset + KeyListener.NoteTime));
                } else break;
            } while (!NewNoteManager.IsTop(++i));
        }

        private IEnumerator ReadTick() {
            while (true) {
                // yield return _seconds;

                if (NewNoteManager.IsTop(0)) continue;
                var now = GetPlayTime();
                var i = 0;
                do {
                    var note = NewNoteManager.Pick(i).note;
                    if (note.offset <= now + 1) {
                        // Debug.Log($"Tick: {note.time}");
                        StartCoroutine(Player.Instance.Accept(NewNoteManager.Pop(), 1));
                    }
                } while (!NewNoteManager.IsTop(++i));
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
using System.Collections;
using Listener;
using Musics.Data;
using UnityEngine;

namespace Musics {
    public class Ticker : SingleMono<Ticker> {
        [SerializeField] private AudioSource beatSound;
        [SerializeField] private AudioSource musicSound;

        private bool _readTick;
        private float _writeTime;

        public void Beat() {
            // if (_beat) return;
            beatSound.PlayOneShot(beatSound.clip);
            // _beat = true;
        }

        public bool IsTickReading() => _readTick;

        public float GetStartTime() => _writeTime;

        public float GetPlayTime() => Time.realtimeSinceStartup - _writeTime;

        public void ResetWrite() => _writeTime = Time.realtimeSinceStartup;

        public void Write() {
            Debug.Log("Write Start");
            _writeTime = Time.realtimeSinceStartup;
            _readTick = true;
            // _routine = StartCoroutine(ReadTick());
            musicSound.PlayOneShot(MusicManager.Instance.GetCurrentMusicData().mainAudio);
        }

        public void StopWrite() {
            Debug.Log("Write Stop");
            if(!_readTick) return;
            _readTick = false;
            // if (_routine is null) return;
            // StopCoroutine(_routine);
            musicSound.Stop();
        }

        public void StopWriteSoftness() {
            Debug.Log("Write Stop");
            if(!_readTick) return;
            _readTick = false;
            // if (_routine is null) return;
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
            if (!_readTick) return;
            if (!MusicManager.Instance.IsPlayMode() || NoteManager.IsTop(0)) return;
            Tick();
        }

        protected virtual void Tick() {
            var now = GetPlayTime();
            var i = 0;
            do {
                var note = NoteManager.Pick(i);
                if (note.time <= now + KeyListener.NoteTime) {
                    StartCoroutine(Player.Instance.Accept(NoteManager.Pop(), note.time - (now + KeyListener.NoteTime)));
                } else break;
            } while (!NoteManager.IsTop(++i));
        }

        private IEnumerator ReadTick() {
            while (true) {
                // yield return _seconds;

                if (!MusicManager.Instance.IsPlayMode() || NoteManager.IsTop(0)) continue;
                var now = GetPlayTime();
                var i = 0;
                do {
                    var note = NoteManager.Pick(i);
                    if (note.time <= now + 1) {
                        // Debug.Log($"Tick: {note.time}");
                        StartCoroutine(Player.Instance.Accept(NoteManager.Pop(), note.time - (now + 1)));
                    }
                } while (!NoteManager.IsTop(++i));
            }
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
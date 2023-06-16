using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Map;
using Musics;
using Musics.Data;
using Score;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace InGame.Listener {
    public class KeyListener : SingleMono<KeyListener> {
        
        [SerializeField] protected Image scoreImage;
        [SerializeField] protected Sprite[] sprites;
        [SerializeField] protected Text increases;
        
        private Sequence _scoreSequence;
        private static readonly Vector3 BeforeScorePos = new(335f, 144f);
        private static readonly Vector3 ScorePos = new(346.5f, 144f);

        private KeyCode _code;

        protected Queue<LiveNoteData>[] noteQueue;
        protected KeyCode[] keyCodes;

        private void Start() {
            SetUp();
            increases.text = "";
            _scoreSequence = DOTween.Sequence()
                .SetAutoKill(false)
                .OnStart(() => {
                    var trans = increases.transform;
                    trans.localPosition = BeforeScorePos;
                    trans.localScale = Vector3.one * 1.05f;
                    increases.color = GameUtils.ClearWhite;
                })
                .Join(increases.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutCubic))
                .Join(increases.transform.DOLocalMove(ScorePos, 0.5f).SetEase(Ease.OutCubic))
                .Join(increases.DOColor(Color.white, 0.5f).SetEase(Ease.OutCubic))
                .OnComplete(() => increases.DOFade(0, 0.5f).SetEase(Ease.OutCubic));
        }

        protected virtual void SetUp() {
            keyCodes = PlayerData.PlayerData.Instance.GetUserData().keyData.keypadKey;
            Debug.Log(keyCodes);
            noteQueue = new Queue<LiveNoteData>[9];
            for (var i = 0; i < 9; i++) noteQueue[i] = new Queue<LiveNoteData>();
        }

        public void Update() {
            if (!MusicManager.Instance.IsPlayMode() && Input.GetKeyDown(KeyCode.Backspace)) {
                Player.Instance.Stop(true);
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                if(!Ticker.Instance.IsTickReading()) return;
                Player.Instance.Stop(false);
                return;
            }
        
            // if (Input.GetKey(KeyCode.R)) {
            //     NoteManager.Stop();
            //     NoteManager.
            //     return;
            // }

            // if (MusicManager.GetCurrentGameMode() == GameMode.Keypad) {
            for (var i = 0; i < keyCodes.Length; i++) {
                _code = keyCodes[i];
                if (!Input.GetKeyDown(_code)) continue;
                Debug.Log($"id: {i}");
                MapMaker.Instance.Click(i);
                Ticker.Instance.Beat();
                if (!MusicManager.Instance.IsPlayMode()) {
                    NoteManager.AddNote(i);
                } else {
                    LiveNoteData liveNoteData = null;
                    while (noteQueue[i].Count > 0 && liveNoteData == null) {
                        liveNoteData = noteQueue[i].Dequeue();
                        if (liveNoteData.clicked) liveNoteData = null;
                    }

                    if (liveNoteData == null) return;
                    var ticker = Ticker.Instance;
                    liveNoteData.Click();
                    var diff = Math.Abs(liveNoteData.time - ticker.GetPlayTime());
                    Debug.Log($"{liveNoteData.time}: {diff}s");
                    Spawn(liveNoteData, NoteManager.GetPerfect(diff));
                }
            }
            // } else {
            //     var quadKey2 = PlayerData.PlayerData.Instance.GetUserData().keyData.quadKey2;
            //     for (var i = 0; i < quadKey2.Length; i++) {
            //         _code = quadKey2[i];
            //         if (!Input.GetKeyDown(_code)) continue;
            //         Debug.Log($"id: {i}");
            //         MapMaker.Instance.Click(i);
            //         Ticker.Instance.Beat();
            //         if (!MusicManager.Instance.IsPlayMode()) {
            //             NoteManager.AddNote(i);
            //         } else {
            //             LiveNoteData liveNoteData = null;
            //             while (noteQueue[i].Count > 0 && liveNoteData == null) {
            //                 liveNoteData = noteQueue[i].Dequeue();
            //                 if (liveNoteData.clicked) liveNoteData = null;
            //             }
            //
            //             if (liveNoteData == null) return;
            //             var ticker = Ticker.Instance;
            //             liveNoteData.Click();
            //             var diff = Math.Abs(liveNoteData.time - ticker.GetPlayTime());
            //             Debug.Log($"{liveNoteData.time}: {diff}s");
            //             Spawn(liveNoteData, NoteManager.GetPerfect(diff));
            //         }
            //     }
            // }
        }

        protected void Spawn(LiveNoteData data, ScoreType score) {
            Debug.Log($"{data.time}, {score.GetTag()}");
            var obj = Instantiate(scoreImage, GameUtils.LocationToCanvas(GameUtils.Locator(MusicManager.GetCurrentGameMode(), data.note)), Quaternion.identity);
            obj.transform.SetParent(GameUtils.Canvas.transform, false);
            obj.sprite = sprites[(int) score];
            increases.text = $"+{Counter.Instance.Count(data.note, score):n0}";
            _scoreSequence.Restart();
        }

        public void Queue(LiveNoteData data) => StartCoroutine(Enqueue(data));

        public static float NoteTime => NoteManager.GetNoteTime();
        public static float AllowedTime => NoteManager.GetNoteAllowedTime();

        protected virtual IEnumerator Enqueue(LiveNoteData data) {
            // while (NoteQueue[data.note].Count > 1) {
            //     NoteQueue[data.note].Dequeue().Click();
            //     Spawn(data, ScoreType.Miss);
            // }
            noteQueue[data.note].Enqueue(data);
            yield return new WaitForSecondsRealtime(AllowedTime * 2);
            if (data.clicked || (noteQueue[data.note].Count != 0 && noteQueue[data.note].Peek().clicked)) yield break;
            data.Click();
            Spawn(data, ScoreType.Miss);
            noteQueue[data.note].Dequeue();
        }
    }
}
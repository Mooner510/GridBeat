using System;
using System.Collections;
using System.Collections.Generic;
using _scenes.InGame.Map;
using _scenes.InGame.Player;
using DG.Tweening;
using Musics;
using Musics.Data;
using Score;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace _scenes.InGame.Listener {
    public class KeyListener : SingleMono<KeyListener> {
        [SerializeField] protected Image scoreImage;
        [SerializeField] protected Sprite[] sprites;
        [SerializeField] protected Text increases;

        private Sequence _scoreSequence;
        private static readonly Vector3 BeforeScorePos = new(335f, 144f);
        private static readonly Vector3 ScorePos = new(346.5f, 144f);

        private KeyCode _code;

        protected Queue<PlayableNote>[] noteQueue;
        protected KeyCode[] keyCodes;

        private void Start() {
            SetUp();
            increases.text = "";
            _scoreSequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
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
            noteQueue = new Queue<PlayableNote>[9];
            for (var i = 0; i < 9; i++) noteQueue[i] = new Queue<PlayableNote>();
        }

        public void Update() {
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Escape)) {
                Player.Player.Instance.Stop();
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
                PlayableNote liveNoteData = null;
                while (noteQueue[i].Count > 0 && liveNoteData == null) {
                    liveNoteData = noteQueue[i].Dequeue();
                    if (liveNoteData.isClicked) liveNoteData = null;
                }

                if (liveNoteData == null) return;
                var ticker = Ticker.Instance;
                liveNoteData.Click();
                var diff = Math.Abs(liveNoteData.note.offset - ticker.GetPlayTime());
                Debug.Log($"{liveNoteData.note.offset}: {diff:#,0}ms");
                Spawn(liveNoteData, NewNoteManager.GetPerfect(diff));
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

        protected void Spawn(PlayableNote data, ScoreType score) {
            Debug.Log($"{data.note.offset}, {score.GetTag()}");
            var obj = Instantiate(scoreImage,
                GameUtils.LocationToCanvas(GameUtils.Locator(NewMusicManager.GetGameMode(), data.note.key)),
                Quaternion.identity);
            obj.transform.SetParent(GameUtils.Canvas.transform, false);
            obj.sprite = sprites[(int) score];
            increases.text = $"+{Counter.Instance.Count(data.note.key, score):n0}";
            _scoreSequence.Restart();
        }

        public void Queue(PlayableNote data) => StartCoroutine(Enqueue(data));

        public static float NoteTime => NewNoteManager.GetNoteTime();
        public static float AllowedTime => NewNoteManager.GetNoteAllowedTime();

        protected virtual IEnumerator Enqueue(PlayableNote data) {
            // while (NoteQueue[data.note].Count > 1) {
            //     NoteQueue[data.note].Dequeue().Click();
            //     Spawn(data, ScoreType.Miss);
            // }
            var note = data.note;
            noteQueue[note.key].Enqueue(data);
            yield return new WaitForSecondsRealtime(AllowedTime * 2);
            if (data.isClicked || (noteQueue[note.key].Count != 0 && noteQueue[note.key].Peek().isClicked)) yield break;
            data.Click();
            Spawn(data, ScoreType.Miss);
            noteQueue[data.note.key].Dequeue();
        }
    }
}
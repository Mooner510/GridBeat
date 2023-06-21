using System.Collections;
using _scenes.InGame.Listener;
using _scenes.InGame.Map;
using DG.Tweening;
using Musics;
using Musics.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace _scenes.InGame.Player {
    public class Player : SingleMono<Player> {
        [SerializeField] protected GameObject beatInspector;
        [SerializeField] private SpriteRenderer hider;
        [SerializeField] private Text recording;

        private static readonly Color ClickColor = new(0f, 0.8f, 0.8f, 0.3f);

        private void Start() {
            if (MusicManager.Instance.IsPlayMode()) {
                NoteManager.LoadCurrentData();
                Debug.Log(NoteManager.GetNoteData().Count);
                Debug.Log("Data Loaded");
                recording.enabled = false;
            } else recording.enabled = true;
            StartCoroutine(Init());
        }

        public void Stop(bool save) {
            StopCoroutine(Init());
            StartCoroutine(End(save));
        }

        private IEnumerator End(bool save) {
            NoteManager.Stop(save);
            var color = hider.color;
            for (var i = 0f; i <= 3; i += Time.deltaTime) {
                color.a = i / 3;
                hider.color = color;
                yield return null;
            }
            hider.color = Color.black;
            SceneManager.LoadScene(MusicManager.Instance.IsPlayMode() ? 3 : 0);
        }

        private IEnumerator Init() {
            var color = hider.color;
            Ticker.Instance.ResetWrite();
            NoteManager.Start(4);
            for (var i = 0f; i <= 3; i += Time.deltaTime) {
                color.a = 1 - i / 3;
                hider.color = color;
                yield return null;
            }
            hider.color = Color.clear;
            yield return new WaitForSecondsRealtime(1);
            var data = MusicManager.Instance.GetCurrentMusicData();
            yield return new WaitForSecondsRealtime(data.minute * 60 + data.second + 1);
            StartCoroutine(End(true));
        }

        protected static IEnumerator Follow(GameObject obj, int note) {
            while (true) {
                if (obj.IsDestroyed()) break;
                obj.transform.position = MapMaker.Instance.GetNote(note).transform.position;
                yield return null;
            }
        }

        public virtual IEnumerator Accept(LiveNoteData note, float time) {
            // // 아에 안 멈출거 같은데?
            // if(time > 0) yield return new WaitForSecondsRealtime(time);
            var obj = Instantiate(beatInspector, GameUtils.Locator(GameMode.Keypad, note.note), Quaternion.identity);
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();
            
            StartCoroutine(Follow(obj, note.note));
            
            obj.transform.DOScale(Vector3.one * 2f, time).SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(time - KeyListener.AllowedTime);
            KeyListener.Instance.Queue(note);
            spriteRenderer.color = ClickColor;
            yield return new WaitForSecondsRealtime(KeyListener.AllowedTime);
            obj.transform.DOScale(Vector3.one * 2.35f, KeyListener.AllowedTime).SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(KeyListener.AllowedTime);
            Destroy(obj);
        }
    }
}
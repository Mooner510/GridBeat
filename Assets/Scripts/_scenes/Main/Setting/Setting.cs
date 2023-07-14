using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Musics.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _scenes.Main.Setting {
    public class Setting : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI noteSpeed;
        [SerializeField] private TMP_InputField inputDelay;

        [SerializeField] private GameObject noteInspector;
        [SerializeField] private TextMeshProUGUI delayText;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip snare;
        [SerializeField] private GameObject parent;

        [SerializeField] private bool keyCheck;

        private Queue<float> _lastTimes;

        private Coroutine _coroutine;
        private Coroutine _fadeCoroutine;

        public static Setting instance;

        private void Start() {
            instance = this;
            noteSpeed.text = $"{NewNoteManager.GetNoteSpeed():#.0}";
            inputDelay.text = $"{NewNoteManager.GetInputDelay()}ms";
            inputDelay.onValueChanged.AddListener(value => {
                if (int.TryParse(value, out var result)) {
                    SetInputDelay(result);
                }
            });
            _lastTimes = new Queue<float>();
        }
        
        public void NoteSpeed(bool isUp) {
            if(isUp) NewNoteManager.NoteSpeedUp(Input.GetKey(KeyCode.LeftShift));
            else NewNoteManager.NoteSpeedDown(Input.GetKey(KeyCode.LeftShift));
            noteSpeed.text = $"{NewNoteManager.GetNoteSpeed():#.0}";
        }
        
        public void InputDelay(bool isUp) {
            if(isUp) SetInputDelay(NewNoteManager.GetInputDelay() + 1);
            else SetInputDelay(NewNoteManager.GetInputDelay() - 1);
        }

        private void SetInputDelay(int value) {
            NewNoteManager.SetInputDelay(Mathf.Clamp(value, -999, 999));
            inputDelay.text = $"{NewNoteManager.GetInputDelay()}ms";
        }

        private void OnEnable() {
            _coroutine = StartCoroutine(RepeatRun());
            _startTime = Time.realtimeSinceStartup;
            _objectQueue = new Queue<GameObject>();
        }

        private void OnDisable() {
            if (_coroutine != null) StopCoroutine(_coroutine);
            if (_objectQueue.Count > 0) {
                foreach (var o in _objectQueue) {
                    Destroy(o);
                }
            }
        }

        private static readonly Color GoodColor = new(0.15f, 0.85f, 0.15f, 1);
        private static readonly Color BadColor = new(0.75f, 0.55f, 0f, 1);
        private static readonly Color TooBadColor = new(0.8f, 0.2f, 0.2f, 1);

        private float _lastTime;
        private float _lastClick;

        private bool CheckJoyStick() => PlayerData.PlayerData.Instance.GetUserData().keyData.quadKey2.Any(Input.GetKey);

        private void Update() {
            if(!Input.anyKeyDown) return;

            if (keyCheck) {
                foreach(KeyCode code in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKey(code)) Debug.Log("KeyCode down: " + code);
                }
            }
            
            if (!CheckJoyStick()) {
                if (Input.inputString == null || Input.inputString.Length <= 0) return;
            }

            _lastTime = Time.realtimeSinceStartup - (_startTime + NewNoteManager.GetNoteAllowedTime());
            if (_lastTimes.Count > 0 && _lastClick != 0 && Time.realtimeSinceStartup - _lastClick > 3) {
                _lastTimes = new Queue<float>();
            }

            _lastClick = Time.realtimeSinceStartup;

            _lastTimes.Enqueue(_lastTime);

            if (_lastTimes.Count >= 6) {
                _lastTimes.Dequeue();
                SetInputDelay(Mathf.FloorToInt( _lastTimes.Average() * 1000));
            }

            var diff = Mathf.FloorToInt(_lastTime * 1000);
            delayText.text = $"{diff:#,0}ms";
            if(_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(Fade(delayText, Mathf.Abs(diff) switch {
                <= 50 => GoodColor,
                <= 130 => BadColor,
                _ => TooBadColor
            }));
        }

        private IEnumerator Fade(Graphic text, Color color) {
            text.color = color;
            for (var i = 1f; i >= 0; i -= Time.deltaTime) {
                var now = text.color;
                now.a = i;
                text.color = now;
                yield return null;
            }
        }

        private float _startTime;

        private static readonly Vector3 EndPos = new(241, -241);
        private static readonly Vector3 StartPos = new(-141, 141);

        private IEnumerator RepeatRun() {
            var start = Time.realtimeSinceStartup;
            var next = 1;
            while (true) {
                yield return null;
                if (!(Time.realtimeSinceStartup - start >= next / 2f)) continue;
                audioSource.PlayOneShot(audioSource.clip);
                if (next % 2 == 0) StartCoroutine(Run());
                next++;
            }
        }

        private Queue<GameObject> _objectQueue;

        private IEnumerator Run() {
            var obj = Instantiate(noteInspector, StartPos, Quaternion.identity, parent.transform);
            _objectQueue.Enqueue(obj);
            var rectTransform = obj.GetComponent<RectTransform>();
            var image = obj.GetComponent<Image>();
            image.color = Color.clear;
            rectTransform.localPosition = StartPos;
            
            rectTransform.DOLocalMove(EndPos, NewNoteManager.GetNoteTime()).SetEase(Ease.Linear);
            image.DOColor(Color.white, 0.1f).SetEase(Ease.Linear);
            
            yield return new WaitForSeconds(NewNoteManager.GetNoteTime() - NewNoteManager.GetNoteAllowedTime());
            
            _startTime = Time.realtimeSinceStartup;

            yield return new WaitForSeconds(NewNoteManager.GetNoteAllowedTime());
            
            audioSource.PlayOneShot(snare);
            Destroy(obj);
            _objectQueue.Dequeue();
        }
    }
}
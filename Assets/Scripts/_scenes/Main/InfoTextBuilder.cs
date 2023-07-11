using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utils;

namespace _scenes.Main {
    public class InfoTextBuilder : MonoBehaviour {
        private static InfoTextBuilder _instance;

        [SerializeField] private GameObject infoText;
        [SerializeField] private Canvas canvas;
        private readonly Queue<GameObject> _textQueue = new();

        private void Awake() {
            if (_instance == null) _instance = this;
            else Destroy(gameObject);
        }

        public static readonly Action<TextMeshProUGUI, float> EaseFadeAndMoveUp = (text, time) => DOTween.Sequence()
            .Join(text.transform.DOLocalMove(new Vector3(0, -120, 0), time).SetEase(Ease.OutCubic))
            .Join(text.DOFade(0, time).SetEase(Ease.OutCubic))
            .Play();

        public static readonly Action<TextMeshProUGUI, float> EaseFade = (text, time) =>
            text.DOFade(0, time).SetDelay(1).SetEase(Ease.OutCubic).Play();

        public static void ShowMessage(string message, Color color, float time, Action<TextMeshProUGUI, float> func) {
            var nextText = GetNextText();
            var textMeshProUGUI = nextText.GetComponent<TextMeshProUGUI>();
            textMeshProUGUI.text = message;
            textMeshProUGUI.color = color;
            func.Invoke(textMeshProUGUI, time);
            ReturnText(textMeshProUGUI.gameObject, time);
        }

        private static GameObject GetNextText() {
            var obj = _instance._textQueue.Count <= 0 ? _instance.NewNote() : _instance._textQueue.Dequeue();
            obj.transform.SetLocalPosition((ref Vector3 vector3) => vector3.y = -160);
            obj.gameObject.SetActive(true);
            return obj;
        }

        private static void ReturnText(GameObject gameObject) {
            gameObject.gameObject.SetActive(false);
            _instance._textQueue.Enqueue(gameObject);
        }

        private static void ReturnText(GameObject gameObject, float time) => _instance._afterReturn(gameObject, time);

        private void _afterReturn(GameObject obj, float after) => StartCoroutine(_afterDestroy(obj, after));

        private static IEnumerator _afterDestroy(GameObject obj, float after) {
            yield return new WaitForSeconds(after);
            ReturnText(obj);
        }

        private GameObject NewNote() {
            var obj = Instantiate(infoText, canvas.transform);
            obj.gameObject.SetActive(false);
            return obj;
        }
    }
}
using System.Collections;
using _scenes.InGame.Map;
using Score;
using UnityEngine;
using UnityEngine.UI;

namespace _scenes.InGame {
    public class Counter : SingleMono<Counter> {
        private static int _combo;
        private static double _score;
        private static int _total;
        private static int[] _data;

        public static int GetCombo() => _combo;
        public static double GetScore() => _score;
        public static int GetTotal() => _total;
        public static int GetData(ScoreType scoreType) => _data[(int) scoreType];

        [SerializeField] private Text comboText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text perfectText;
        // [SerializeField] private Text[] texts;
        private int[] _beforeSize;

        public void Start() {
            _score = 0d;
            _combo = 0;
            _total = 0;
            _data = new int[5];
            comboText.text = "";
            scoreText.text = "0";
            perfectText.text = "0.00%";
            _beforeSize = new[] {comboText.fontSize, scoreText.fontSize, perfectText.fontSize};
        }

        private IEnumerator Sizing(Text text, int index) {
            var before = _beforeSize[index];
            for (var i = 0f; i <= 0.5f; i += Time.deltaTime) {
                yield return null;
                text.fontSize = (int) (before * (1 - i / 4));
            }
            text.transform.localScale = Vector3.one;
        }

        public double Count(int note, ScoreType scoreType) {
            _total++;
            var value = (int) scoreType;
            
            if (value < 2) _combo++;
            else _combo = 0;

            _data[(int) scoreType]++;
            comboText.text = _combo > 0 ? $"{_combo}" : "";
            
            if(scoreType == ScoreType.Miss) return 0;
            var baseScore = scoreType.GetBaseScore() * (1 + _combo / 200d);
            scoreText.text = $"{_score += baseScore:n0}";
            perfectText.text = $"{_data[0] * 100f / _total:n0}%";
            
            StopCoroutine(Sizing(comboText, 0));
            StartCoroutine(Sizing(comboText, 0));

            StopCoroutine(Sizing(scoreText, 1));
            StartCoroutine(Sizing(scoreText, 1));
            
            StopCoroutine(Sizing(perfectText, 2));
            StartCoroutine(Sizing(perfectText, 2));

            StopCoroutine(MapMaker.Instance.Beat(note));
            StartCoroutine(MapMaker.Instance.Beat(note));
            return baseScore;
        }
    }
}
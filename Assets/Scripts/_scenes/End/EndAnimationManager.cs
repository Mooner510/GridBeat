﻿using System;
using System.Collections;
using _scenes.InGame;
using DG.Tweening;
using Musics;
using Resource;
using Score;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace _scenes.End {
    public class EndAnimationManager : SingleMono<EndAnimationManager> {
        [SerializeField] private Image image;
        [SerializeField] private Text newRecord;
        [SerializeField] private Text score;
        [SerializeField] private Text maxScore;
        [SerializeField] private Text perfectPercent;
        [SerializeField] private Text finalPercent;
        [SerializeField] private Text perfect;
        [SerializeField] private Text great;
        [SerializeField] private Text good;
        [SerializeField] private Text bad;
        [SerializeField] private Text miss;
        [SerializeField] private Text artist;
        [SerializeField] private Text difficulty;
        [SerializeField] private Image rank;
        [SerializeField] private AudioSource audioPlayer;
        [SerializeField] private Button button;

        private void Start() {
            newRecord.enabled = false;
            rank.enabled = false;
            rank.color = GameUtils.ClearWhite;
            SetUp();
            StartCoroutine(Animation());
            button.onClick.AddListener(() => SceneManager.LoadScene("Scenes/NewMain"));
        }

        private void SetUp() {
            var musicData = NewMusicManager.Instance.GetMusicData();
            audioPlayer.clip = musicData.previewAudio;
            audioPlayer.Play();
            var id = NewMusicManager.Instance.GetMusicId();
            var max = PlayerPrefs.HasKey($"max:{id}") ? PlayerPrefs.GetFloat($"max:{id}") : 0;
            if (max < Counter.GetScore()) {
                PlayerPrefs.SetFloat($"max:{id}", (float) Counter.GetScore());
                newRecord.enabled = true;
                DOTween.Sequence()
                    .OnStart(() => {
                        newRecord.color = GameUtils.ClearWhite;
                        rank.transform.localScale = Vector3.one * 2f;
                    })
                    .Join(rank.DOFade(1, 3).SetEase(Ease.OutCubic))
                    .Join(rank.transform.DOScale(1, 3).SetEase(Ease.OutCubic))
                    .Play();
            }

            artist.DOText(musicData.musicInfo.name, .75f).SetEase(Ease.OutCubic).Play();
            difficulty.DOText(NewMusicManager.GetDifficulty().ToString(), .75f).SetEase(Ease.OutCubic).Play();
            image.sprite = musicData.blurImage;
            maxScore.text = $"{max:n0}";
        }

        private IEnumerator Animation() {
            var musicData = NewMusicManager.Instance.GetMusicData();
            var gameMode = NewMusicManager.GetGameMode();
            perfect.text = "";
            great.text = "";
            good.text = "";
            bad.text = "";
            miss.text = "";
            score.text = "";
            perfectPercent.text = "";
            finalPercent.text = "";
            
            var perfectCount = Counter.GetData(ScoreType.Perfect);
            var greatCount = Counter.GetData(ScoreType.Great);
            var goodCount = Counter.GetData(ScoreType.Good);
            var badCount = Counter.GetData(ScoreType.Bad);
            var missCount = Counter.GetData(ScoreType.Miss);
            var noteData = musicData.GetMapData();
            var totalCount = noteData.notes.Length;
            var currentScore = (float) Counter.GetScore();
            var final = (perfectCount + greatCount * 0.75f + goodCount * 0.25f + badCount * 0.025f) * 75 / totalCount + perfectCount * 25f / totalCount;
            var finalIndex = 9 - ((int) (Math.Round(final) / 10)).Between(9, 0);
            var increases = perfectCount * 100f / totalCount;
            var values = new float[8];
            
            rank.sprite = ResourceManager.Instance.GetRank(finalIndex);
            score.color = ResourceManager.Instance.GetRankColor(finalIndex);
            perfectPercent.color = ResourceManager.Instance.GetRankColor(finalIndex);
            var localY = rank.transform.localPosition.y;
            rank.transform.DOLocalMoveY(localY + 6, 2f);

            for (var i = 0f; i <= 1; i += Time.deltaTime) {
                var delta = Time.deltaTime;
                perfect.text = $"{values[0] += perfectCount * delta:n0}";
                great.text = $"{values[1] += greatCount * delta:n0}";
                good.text = $"{values[2] += goodCount * delta:n0}";
                bad.text = $"{values[3] += badCount * delta:n0}";
                miss.text = $"{values[4] += missCount * delta:n0}";
                score.text = $"{values[5] += currentScore * delta:n0}";
                perfectPercent.text = $"{values[6] += increases * delta:n2}%";
                finalPercent.text = $"{values[7] += final * delta:n2}%";
                yield return null;
            }
            rank.enabled = true;
            DOTween.Sequence()
                .OnStart(() => rank.transform.localScale = Vector3.one * 1.5f)
                .Append(rank.DOFade(1, 2).SetEase(Ease.OutCubic))
                .Join(rank.transform.DOScale(1, 2).SetEase(Ease.OutCubic))
                .Play();
            perfect.text = $"{perfectCount:n0}";
            great.text = $"{greatCount:n0}";
            good.text = $"{goodCount:n0}";
            bad.text = $"{badCount:n0}";
            miss.text = $"{missCount:n0}";
            score.text = $"{currentScore:n0}";
            perfectPercent.text = $"{perfectCount * 100f / totalCount:n2}%";
            finalPercent.text = $"{final:n2}%";
            yield return new WaitForSecondsRealtime(1f);
            DOTween.Sequence()
                .Append(rank.transform.DOLocalMoveY(localY - 6, 2f))
                .Append(rank.transform.DOLocalMoveY(localY + 6, 2f))
                .SetLoops(-1)
                .Play();
        }
    }
}
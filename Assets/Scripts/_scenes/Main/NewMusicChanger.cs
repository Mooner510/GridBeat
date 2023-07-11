using System;
using System.Collections;
using DG.Tweening;
using Musics;
using Musics.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace _scenes.Main {
    public class NewMusicChanger : SingleMono<NewMusicChanger> {
        [Header("Buttons")]
        [SerializeField] private Image difficultyImage;
        [SerializeField] private Image gameModeImage;

        [Header("Music Info")]
        [SerializeField] private Image musicLevelImage;

        [SerializeField] private TextMeshProUGUI musicNameText;
        [SerializeField] private TextMeshProUGUI musicArtistText;
        [SerializeField] private TextMeshProUGUI musicDurationText;
        [SerializeField] private Image[] musicImages;

        [Header("Images")]
        [SerializeField] private Sprite[] musicLevelImages;
        [SerializeField] private Sprite[] difficultyImages;
        [SerializeField] private Sprite[] gameModeImages;

        [Header("Utils")]
        [SerializeField] private SpriteRenderer hider;
        [SerializeField] private AudioSource audioPlayer;

        [Header("Prefabs")]
        [SerializeField] private GameObject image;

        #region Location

        private static readonly Vector3[] ImageLocations = {
            new(-1200, 40),
            new(-1000, 40),
            new(-570, 80),
            new(0, 100),
            new(570, 80),
            new(1000, 40),
            new(1200, 40)
        };

        private static readonly Vector2[] ImageSizeDelta = {
            new(300, 300),
            new(300, 300),
            new(450, 450),
            new(600, 600),
            new(450, 450),
            new(300, 300),
            new(300, 300)
        };

        private static readonly Vector3 DifficultyLocation = new(-12, -12);
        private static readonly Vector3 GameModeLocation = new(12, -12);

        #endregion
        
        private bool _canStart;
        private Sequence[] _sequences;

        private void Start() {
            hider.color = Color.clear;
            audioPlayer.volume = 1;

            for (var i = -3; i <= 3; i++) {
                var musicData = NewMusicManager.Instance.GetMusicData(i);
                musicImages[i + 3].sprite = musicData.image;
            }

            Refresh(NewMusicManager.Instance.GetMusicData());
        }

        private void TextUpdate(
            bool isRight
        ) {
            NewMusicData musicData;
            if (isRight) {
                musicData = NewMusicManager.Instance.Next();
                for (var i = -2; i <= 2; i++) {
                    var rectTransform = musicImages[i + 3].rectTransform;
                    rectTransform.DOLocalMove(ImageLocations[i + 4], 0.6f).SetEase(Ease.OutCubic);
                    rectTransform.DOSizeDelta(ImageSizeDelta[i + 4], 0.6f).SetEase(Ease.OutCubic);
                }

                Destroy(musicImages[0].gameObject, 0.6f);
                var copy = new Image[musicImages.Length];
                Array.Copy(musicImages, 0, copy, 1, musicImages.Length - 1);
                var newImage = Instantiate(image, ImageLocations[0], Quaternion.identity, GameUtils.Canvas)
                    .GetComponent<Image>();
                newImage.sprite = NewMusicManager.Instance.GetMusicData(-3).image;
                copy[0] = newImage;
                musicImages = copy;
            } else {
                musicData = NewMusicManager.Instance.Back();
                for (var i = -2; i <= 2; i++) {
                    var rectTransform = musicImages[i + 3].rectTransform;
                    rectTransform.DOLocalMove(ImageLocations[i + 2], 0.6f).SetEase(Ease.OutCubic);
                    rectTransform.DOSizeDelta(ImageSizeDelta[i + 2], 0.6f).SetEase(Ease.OutCubic);
                }

                Destroy(musicImages[^1].gameObject, 0.6f);
                var copy = new Image[musicImages.Length];
                Array.Copy(musicImages, 1, copy, 0, musicImages.Length - 1);
                var newImage = Instantiate(image, ImageLocations[^1], Quaternion.identity, GameUtils.Canvas)
                    .GetComponent<Image>();
                newImage.sprite = NewMusicManager.Instance.GetMusicData(3).image;
                copy[^1] = newImage;
                musicImages = copy;
            }

            Refresh(musicData);
        }

        private void Refresh(NewMusicData musicData) {
            var difficulty = NewMusicManager.GetDifficulty();
            musicNameText.text = musicData.musicInfo.name;
            musicArtistText.text = musicData.musicInfo.artist;
            musicDurationText.text = $"{musicData.musicInfo.playTime / 60:00}:{musicData.musicInfo.playTime % 60:00}";
            musicLevelImage.sprite = musicLevelImages[musicData.mapData[difficulty].level - 1];
            difficultyImage.sprite = difficultyImages[(int) difficulty];

            audioPlayer.Stop();
            audioPlayer.clip = musicData.previewAudio;
            audioPlayer.Play();
        }

        private IEnumerator StartMusic(GameMode gameMode) {
            audioPlayer.DOFade(0, 3);
            yield return new WaitForSecondsRealtime(1);
            hider.DOColor(Color.black, 2).SetEase(Ease.OutCubic);
            yield return new WaitForSecondsRealtime(3);
            switch (gameMode) {
                case GameMode.Keypad:
                    SceneManager.LoadScene(1);
                    break;
                case GameMode.Quad:
                    SceneManager.LoadScene(2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
            }
        }

        private static readonly Color Unshift = new(0.4f, 0.4f, 0.4f);
        private static readonly Color Shift = new(0.7128526f, 1f, 0.4198113f);
        private static readonly Color SuperColor = new(1f, 0.4271304f, 0.3820755f);
        private static readonly Color DefaultColor = new(0.6980392f, 0.6980392f, 0.6980392f);

        private static readonly Color[] SpeedColors = {
            new(0.2044074f, 1f, 0.272549f),
            new(1f, 1f, 0.36f),
            new(1f, 0.57f, 0.1f),
            new(1f, 0.36f, 0.36f),
            new(0.572549f, 0.8310416f, 1f),
            new(0.5f, 0.56f, 1f),
            new(0.77983f, 0.5f, 1f),
            new(1f, 0.37f, 0.76f),
            new(1f, 0.37f, 0.76f)
        };

        private void Update() {
            // if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
            //     speedUp.text = "X ++";
            //     speedDown.text = "-- Z";
            //     speedUp.color = speedDown.color = SuperColor;
            //     shiftText.color = Shift;
            // }
            //
            // if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) {
            //     speedUp.text = "X +";
            //     speedDown.text = "- Z";
            //     speedUp.color = speedDown.color = DefaultColor;
            //     shiftText.color = Unshift;
            // }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                TextUpdate(false);
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                TextUpdate(true);
            }

            // } else {
                var gameMode = NewMusicManager.GetGameMode();
                // if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ||
                //     Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                //     if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) _sequences[2].Restart();
                //     else _sequences[3].Restart();
                //     MusicManager.SetGameMode(gameMode = gameMode == GameMode.Keypad ? GameMode.Quad : GameMode.Keypad);
                //     modeText.color = gameMode.GetColor();
                //     modeText.text = $"{gameMode.ToString()} Mode";
                //     keypadImage.enabled = gameMode == GameMode.Keypad;
                //     quadImage.enabled = gameMode == GameMode.Quad;
                //     UpdateSuggestion(MusicManager.Instance.GetCurrentMusicData());
                // } else if (Input.GetKeyDown(KeyCode.Z)) {
                //     NoteManager.NoteSpeedDown(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                //     speedText.text = $"{NoteManager.GetNoteSpeed():F1}";
                //     speedText.color = SpeedColors[(int) NoteManager.GetNoteSpeed() / 2];
                //     _sequences[5].Restart();
                // } else if (Input.GetKeyDown(KeyCode.X)) {
                //     NoteManager.NoteSpeedUp(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                //     speedText.text = $"{NoteManager.GetNoteSpeed():F1}";
                //     speedText.color = SpeedColors[(int) NoteManager.GetNoteSpeed() / 2];
                //     _sequences[4].Restart();
                
                if (Input.GetKeyDown(KeyCode.E)) {
                    StopCoroutine(StartMusic(gameMode));
                    StartCoroutine(StartMusic(gameMode));
                }
            // }
        }
    }
}
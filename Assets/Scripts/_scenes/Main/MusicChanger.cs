using System;
using System.Collections;
using DG.Tweening;
using JetBrains.Annotations;
using Musics;
using Musics.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace Main {
    public class MusicChanger : SingleMono<MusicChanger> {
        [SerializeField] private Image image;
        [SerializeField] private Text title;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Text left;
        [SerializeField] private Text right;
        [SerializeField] private Text artist;
        [SerializeField] private Text artistInfo;
        [SerializeField] private Text arranger;
        [SerializeField] private Text arrangeInfo;
        [SerializeField] private Text duration;
        [SerializeField] private Text durationInfo;
        [SerializeField] private Text difficulty;
        [SerializeField] private Text difficultyInfo;
        [SerializeField] private Text suggestion1;
        [SerializeField] private Text suggestion2;
        [SerializeField] private Text suggestion3;
        [SerializeField] private Text selectorUp;
        [SerializeField] private Text selectorDown;
        [SerializeField] private Text modeText;
        [SerializeField] private Text speedUp;
        [SerializeField] private Text speedDown;
        [SerializeField] private Text speedText;
        [SerializeField] private Text shiftText;
        [SerializeField] private Text speedInfoText;
        [SerializeField] private Image keypadImage;
        [SerializeField] private Image quadImage;
        [SerializeField] private SpriteRenderer hider;
        [SerializeField] private AudioSource audioPlayer;

        #region Location

        private static readonly Vector3 ImageLocation = new(0, 45);
        private static readonly Vector3 LeftImageLocation = new(-650, 45);
        private static readonly Vector3 RightImageLocation = new(650, 45);
        private static readonly Vector3 TitleLocation = new(0, -100);
        private static readonly Vector3 LeftTitleLocation = new(-650, -100);
        private static readonly Vector3 RightTitleLocation = new(650, -100);

        private const float UIOut = -630f;
        private const float TextOut = UIOut + 138.76f;
        private const float Suggest1Out = -300f;
        private const float Suggest2Out = Suggest1Out - 22f;
        private const float Suggest3Out = Suggest2Out - 17f;
        private const float TitleOut = Suggest1Out + 35f;
        private const float LeftOut = -440f;
        private const float RightOut = 420f;
        private const float SelectorOut = 460f;
        private const float SpeedUpOut = -450f;
        private const float SpeedOut = -531f;
        private const float SpeedDownOut = -606f;

        #endregion

        #region Privates

        private Image _subImage;
        private Text _subTitle;
        private bool _canStart;
        private Sequence _clickSequence;
        private Sequence[] _sequences;

        #endregion

        private void Start() {
            hider.color = Color.clear;
            audioPlayer.volume = 1;
            _canStart = false;

            speedText.text = $"{NoteManager.GetNoteSpeed():F1}";
            speedText.color = SpeedColors[(int) NoteManager.GetNoteSpeed() / 2];

            _clickSequence = DOTween.Sequence()
                .SetAutoKill(false)
                .OnStart(() => suggestion2.transform.localScale = Vector3.one * 1.2f)
                .Append(suggestion2.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutCubic));
            _sequences = new[] {
                DOTween.Sequence()
                    .SetAutoKill(false)
                    .OnStart(() => left.transform.localScale = Vector3.one * 1.2f)
                    .Append(left.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutCubic)),
                DOTween.Sequence()
                    .SetAutoKill(false)
                    .OnStart(() => right.transform.localScale = Vector3.one * 1.2f)
                    .Append(right.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutCubic)),
                DOTween.Sequence()
                    .SetAutoKill(false)
                    .OnStart(() => selectorUp.transform.localScale = Vector3.one * 1.2f)
                    .Append(selectorUp.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutCubic)),
                DOTween.Sequence()
                    .SetAutoKill(false)
                    .OnStart(() => selectorDown.transform.localScale = Vector3.one * 1.2f)
                    .Append(selectorDown.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutCubic)),
                DOTween.Sequence()
                    .SetAutoKill(false)
                    .OnStart(() => speedUp.transform.localScale = Vector3.one * 1.2f)
                    .Append(speedUp.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutCubic)),
                DOTween.Sequence()
                    .SetAutoKill(false)
                    .OnStart(() => speedDown.transform.localScale = Vector3.one * 1.2f)
                    .Append(speedDown.transform.DOScale(Vector3.one, 1).SetEase(Ease.OutCubic)),
            };

            var musicData = MusicManager.Instance.GetCurrentMusicData();

            _subImage = Instantiate(image, ImageLocation, Quaternion.identity);
            _subImage.transform.SetParent(GameUtils.Canvas, false);
            _subImage.sprite = musicData.image;

            _subTitle = Instantiate(title, TitleLocation, Quaternion.identity);
            _subTitle.transform.SetParent(GameUtils.Canvas, false);
            _subTitle.text = musicData.name;

            var gameMode = MusicManager.GetCurrentGameMode();
            keypadImage.enabled = gameMode == GameMode.Keypad;
            quadImage.enabled = gameMode == GameMode.Quad;
            modeText.color = gameMode.GetColor();
            modeText.text = $"{gameMode.ToString()} Mode";

            TextUpdate(null, null, null, null, false, musicData);
        }

        private void TextUpdate([CanBeNull] Image newImage, [CanBeNull] Text newTitle,
            [CanBeNull] Component currentImage, [CanBeNull] Component currentTitle, bool isLeft, MusicData musicData) {
            if (newImage is not null) {
                newImage.sprite = musicData.image;
                newImage.transform.DOLocalMove(ImageLocation, 1f).SetEase(Ease.OutCubic);
            }

            if (newTitle is not null) {
                newTitle.text = musicData.name;
                newTitle.transform.DOLocalMove(TitleLocation, 1f).SetEase(Ease.OutCubic);
            }

            if (currentImage is not null)
                currentImage.transform.DOLocalMove(isLeft ? RightImageLocation : LeftImageLocation, 1f)
                    .SetEase(Ease.OutCubic);
            if (currentTitle is not null)
                currentTitle.transform.DOLocalMove(isLeft ? RightTitleLocation : LeftTitleLocation, 1f)
                    .SetEase(Ease.OutCubic);
            backgroundImage.sprite = musicData.blurImage;

            artist.text = musicData.artist;
            if (musicData.arrange == null) {
                arranger.enabled = false;
                arrangeInfo.enabled = false;
            } else {
                arranger.text = musicData.arrange;
                arranger.enabled = true;
                arrangeInfo.enabled = true;
            }

            UpdateSuggestion(musicData);

            duration.text = $"{musicData.minute}:{musicData.second:00}";
            difficulty.text = $"{StringUtils.ToRoman(musicData.difficulty)}";
            difficulty.color = musicData.GetDifficultyColor();

            audioPlayer.Stop();
            audioPlayer.clip = musicData.titleAudio;
            audioPlayer.Play();
        }

        private void UpdateSuggestion(MusicData musicData) {
            var noteData = musicData.GetNoteData(MusicManager.GetCurrentGameMode());
            if (noteData == null || noteData.Length <= 0) {
                suggestion1.text = "Press E to Record";
                suggestion2.text = "This music doesn't have note map!";
                _canStart = false;
            } else {
                suggestion1.text = "Press Q / Enter to Start";
                suggestion2.text = "Or Press E to Edit";
                _canStart = true;
            }
        }

        private IEnumerator MoveLeft() {
            var musicData = MusicManager.Instance.Back();
            var currentImage = _subImage;
            var currentTitle = _subTitle;
            var newImage = Instantiate(image, LeftImageLocation, Quaternion.identity);
            var newTitle = Instantiate(title, LeftTitleLocation, Quaternion.identity);
            newImage.transform.SetParent(GameUtils.Canvas.transform, false);
            newTitle.transform.SetParent(GameUtils.Canvas.transform, false);

            _subImage = newImage;
            _subTitle = newTitle;

            TextUpdate(newImage, newTitle, currentImage, currentTitle, true, musicData);

            yield return new WaitForSeconds(1f);

            Destroy(currentImage.gameObject);
            Destroy(currentTitle.gameObject);
        }

        private IEnumerator MoveRight() {
            var musicData = MusicManager.Instance.Next();
            var currentImage = _subImage;
            var currentTitle = _subTitle;
            var newImage = Instantiate(image, RightImageLocation, Quaternion.identity);
            var newTitle = Instantiate(title, RightTitleLocation, Quaternion.identity);
            newImage.transform.SetParent(GameUtils.Canvas.transform, false);
            newTitle.transform.SetParent(GameUtils.Canvas.transform, false);

            _subImage = newImage;
            _subTitle = newTitle;

            TextUpdate(newImage, newTitle, currentImage, currentTitle, false, musicData);

            yield return new WaitForSeconds(1f);

            Destroy(currentImage.gameObject);
            Destroy(currentTitle.gameObject);
        }

        private IEnumerator StartMusic(GameMode gameMode) {
            artistInfo.transform.DOLocalMoveX(UIOut, 2).SetEase(Ease.OutCubic);
            arrangeInfo.transform.DOLocalMoveX(UIOut, 2).SetEase(Ease.OutCubic);
            durationInfo.transform.DOLocalMoveX(UIOut, 2).SetEase(Ease.OutCubic);
            difficultyInfo.transform.DOLocalMoveX(UIOut, 2).SetEase(Ease.OutCubic);
            artist.transform.DOLocalMoveX(TextOut, 2).SetEase(Ease.OutCubic);
            arranger.transform.DOLocalMoveX(TextOut, 2).SetEase(Ease.OutCubic);
            duration.transform.DOLocalMoveX(TextOut, 2).SetEase(Ease.OutCubic);
            difficulty.transform.DOLocalMoveX(TextOut, 2).SetEase(Ease.OutCubic);
            suggestion1.transform.DOLocalMoveY(Suggest1Out, 2).SetEase(Ease.OutCubic);
            suggestion2.transform.DOLocalMoveY(Suggest2Out, 2).SetEase(Ease.OutCubic);
            suggestion3.transform.DOLocalMoveY(Suggest3Out, 2).SetEase(Ease.OutCubic);
            selectorUp.transform.DOLocalMoveX(SelectorOut, 2).SetEase(Ease.OutCubic);
            selectorDown.transform.DOLocalMoveX(SelectorOut, 2).SetEase(Ease.OutCubic);
            modeText.transform.DOLocalMoveX(SelectorOut, 2).SetEase(Ease.OutCubic);
            difficultyInfo.transform.DOLocalMoveX(UIOut, 2).SetEase(Ease.OutCubic);
            left.transform.DOLocalMoveX(LeftOut, 2).SetEase(Ease.OutCubic);
            right.transform.DOLocalMoveX(RightOut, 2).SetEase(Ease.OutCubic);
            _subImage.transform.DOLocalMove(Vector3.zero, 3).SetEase(Ease.OutCubic);
            _subImage.transform.DOScale(1.75f, 3).SetEase(Ease.OutCubic);
            _subTitle.transform.DOLocalMoveY(TitleOut, 2).SetEase(Ease.OutCubic);
            speedUp.transform.DOLocalMoveX(SpeedUpOut, 2).SetEase(Ease.OutCubic);
            speedDown.transform.DOLocalMoveX(SpeedDownOut, 2).SetEase(Ease.OutCubic);
            speedText.transform.DOLocalMoveX(SpeedOut, 2).SetEase(Ease.OutCubic);
            shiftText.transform.DOLocalMoveX(SpeedOut, 2).SetEase(Ease.OutCubic);
            speedInfoText.transform.DOLocalMoveX(SpeedOut, 2).SetEase(Ease.OutCubic);

            keypadImage.transform.DOLocalMoveX(SelectorOut, 2).SetEase(Ease.OutCubic);
            quadImage.transform.DOLocalMoveX(SelectorOut, 2).SetEase(Ease.OutCubic);
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

        private static readonly Color Unshift = new Color(0.4f, 0.4f, 0.4f);
        private static readonly Color Shift = new Color(0.7128526f, 1f, 0.4198113f);
        private static readonly Color SuperColor = new Color(1f, 0.4271304f, 0.3820755f);
        private static readonly Color DefaultColor = new Color(0.6980392f, 0.6980392f, 0.6980392f);

        private static readonly Color[] SpeedColors = {
            new Color(0.2044074f, 1f, 0.272549f),
            new Color(1f, 1f, 0.36f),
            new Color(1f, 0.57f, 0.1f),
            new Color(1f, 0.36f, 0.36f),
            new Color(0.572549f, 0.8310416f, 1f),
            new Color(0.5f, 0.56f, 1f),
            new Color(0.77983f, 0.5f, 1f),
            new Color(1f, 0.37f, 0.76f),
            new Color(1f, 0.37f, 0.76f)
        };

        private void Update() {
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
                speedUp.text = "X ++";
                speedDown.text = "-- Z";
                speedUp.color = speedDown.color = SuperColor;
                shiftText.color = Shift;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift)) {
                speedUp.text = "X +";
                speedDown.text = "- Z";
                speedUp.color = speedDown.color = DefaultColor;
                shiftText.color = Unshift;
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                StopCoroutine(MoveLeft());
                StartCoroutine(MoveLeft());
                _sequences[0].Restart();
            } else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                StopCoroutine(MoveRight());
                StartCoroutine(MoveRight());
                _sequences[1].Restart();
            } else {
                var gameMode = MusicManager.GetCurrentGameMode();
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) ||
                    Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) _sequences[2].Restart();
                    else _sequences[3].Restart();
                    MusicManager.SetGameMode(gameMode = gameMode == GameMode.Keypad ? GameMode.Quad : GameMode.Keypad);
                    modeText.color = gameMode.GetColor();
                    modeText.text = $"{gameMode.ToString()} Mode";
                    keypadImage.enabled = gameMode == GameMode.Keypad;
                    quadImage.enabled = gameMode == GameMode.Quad;
                    UpdateSuggestion(MusicManager.Instance.GetCurrentMusicData());
                } else if (Input.GetKeyDown(KeyCode.Z)) {
                    NoteManager.NoteSpeedDown(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                    speedText.text = $"{NoteManager.GetNoteSpeed():F1}";
                    speedText.color = SpeedColors[(int) NoteManager.GetNoteSpeed() / 2];
                    _sequences[5].Restart();
                } else if (Input.GetKeyDown(KeyCode.X)) {
                    NoteManager.NoteSpeedUp(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
                    speedText.text = $"{NoteManager.GetNoteSpeed():F1}";
                    speedText.color = SpeedColors[(int) NoteManager.GetNoteSpeed() / 2];
                    _sequences[4].Restart();
                } else if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Return)) {
                    if (!_canStart) {
                        _clickSequence.Restart();
                        return;
                    }

                    MusicManager.Instance.SetPlayMode(true);
                    MusicManager.Instance.SetPlayMode(true);
                    StopCoroutine(StartMusic(gameMode));
                    StartCoroutine(StartMusic(gameMode));
                } else if (Input.GetKeyDown(KeyCode.E)) {
                    MusicManager.Instance.SetPlayMode(false);
                    StopCoroutine(StartMusic(gameMode));
                    StartCoroutine(StartMusic(gameMode));
                }
            }
        }
    }
}
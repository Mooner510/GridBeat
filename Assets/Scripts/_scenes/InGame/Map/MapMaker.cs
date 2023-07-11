using System.Collections;
using Musics;
using Musics.Data;
using UnityEngine;
using Utils;

namespace _scenes.InGame.Map {
    public class MapMaker : SingleMono<MapMaker> {
        protected GameObject[] notes;
        protected SpriteRenderer[] noteRenderers;
        [SerializeField] protected GameObject beatButton;
        [SerializeField] protected GameObject beatBackButton;
        [SerializeField] protected Sprite[] noteSprites;

        protected Coroutine[] routines;

        private void Start() {
            notes = new GameObject[9];
            noteRenderers = new SpriteRenderer[9];
            routines = new Coroutine[9];
            for (var i = 0; i < 9; i++) {
                (noteRenderers[i] = (notes[i] = Instantiate(beatButton, GameUtils.Locator(GameMode.Keypad, i), Quaternion.identity))
                    .GetComponent<SpriteRenderer>()).sprite = noteSprites[i];
            }
        }

        public GameObject GetNote(int index) => notes[index];

        public void Click(int note) {
            if(routines[note] != null) StopCoroutine(routines[note]);
            routines[note] = StartCoroutine(Clicking(note));
        }

        private IEnumerator Clicking(int note) {
            noteRenderers[note].color = Color.yellow;
            for (var i = 0f; i <= 0.5f; i += Time.deltaTime) {
                yield return null;
                noteRenderers[note].color = Color.yellow + (Color.white - Color.yellow) * i * 2;
            }
            noteRenderers[note].color = Color.white;
        }

        public virtual IEnumerator Beat(int note) {
            for (var time = 0f; time <= 0.5f; time += Time.deltaTime) {
                for (var i = 0; i < 9; i++) {
                    var pos = GameUtils.Locator(NewMusicManager.GetGameMode(), i);
                    notes[i].transform.localPosition = pos * (1 + (0.5f - time) / 8);
                    notes[i].transform.localScale = Vector3.one * (2 + (0.5f - time) / 3);
                }
                yield return null;
            }
        }
    }
}
using System.Collections;
using Musics.Data;
using UnityEngine;
using Utils;

namespace _scenes.InGame.Map {
    public class QuadMapMaker : MapMaker {
        private void Start() {
            notes = new GameObject[4];
            noteRenderers = new SpriteRenderer[4];
            backNotes = new GameObject[4];
            backNoteRenderers = new SpriteRenderer[4];
            routines = new Coroutine[4];
            for (var i = 0; i < 4; i++) {
                backNoteRenderers[i] = (backNotes[i] = Instantiate(beatBackButton, GameUtils.Locator(GameMode.Quad, i), Quaternion.identity))
                    .GetComponent<SpriteRenderer>();
                noteRenderers[i] = (notes[i] = Instantiate(beatButton, GameUtils.Locator(GameMode.Quad, i), Quaternion.identity))
                    .GetComponent<SpriteRenderer>();
            }
        }

        public override IEnumerator Beat(int note) {
            for (var time = 0f; time <= 0.5f; time += Time.deltaTime) {
                for (var i = 0; i < 4; i++) {
                    var pos = GameUtils.Locator(GameMode.Quad, i);
                    notes[i].transform.localPosition = pos * (1 + (0.5f - time) / 8);
                    notes[i].transform.localScale = Vector3.one * (2 + (0.5f - time) / 3);
                    backNotes[i].transform.localPosition = pos * (1 + (0.5f - time) / 8);
                    backNotes[i].transform.localScale = Vector3.one * (0.4f - time * 0.45f / 3);
                }
                yield return null;
            }
        }
    }
}
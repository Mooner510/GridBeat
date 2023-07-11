using System.Collections;
using Musics.Data;
using UnityEngine;
using Utils;

namespace _scenes.InGame.Map {
    public class QuadMapMaker : MapMaker {
        private void Start() {
            notes = new GameObject[4];
            noteRenderers = new SpriteRenderer[4];
            routines = new Coroutine[4];
            for (var i = 0; i < 4; i++) {
                (noteRenderers[i] = (notes[i] = Instantiate(beatButton, GameUtils.Locator(GameMode.Quad, i), Quaternion.identity))
                    .GetComponent<SpriteRenderer>()).sprite = noteSprites[i];
            }
        }

        public override IEnumerator Beat(int note) {
            for (var time = 0f; time <= 0.5f; time += Time.deltaTime) {
                for (var i = 0; i < 4; i++) {
                    var pos = GameUtils.Locator(GameMode.Quad, i);
                    notes[i].transform.localPosition = pos * (1 + (0.5f - time) / 8);
                    notes[i].transform.localScale = Vector3.one * (2 + (0.5f - time) / 3);
                }
                yield return null;
            }
        }
    }
}
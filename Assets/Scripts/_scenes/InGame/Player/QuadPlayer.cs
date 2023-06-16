using System.Collections;
using DG.Tweening;
using InGame.Listener;
using Musics.Data;
using UnityEngine;
using Utils;

namespace Musics {
    public class QuadPlayer : Player {

        private static readonly Color ClickColor = new(0f, 0.8f, 0.8f, 0.5f);
        
        public override IEnumerator Accept(LiveNoteData note, float time) {
            // if(time > 0) yield return new WaitForSecondsRealtime(time);
            
            // var noteTime = KeyListener.NoteTime / 2 - KeyListener.AllowedTime;
            
            var location = GameUtils.Locator(GameMode.Quad, note.note);
            var obj = Instantiate(beatInspector, location * 5f, Quaternion.identity);
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();
            obj.transform.DOMove(location, time).SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(time - KeyListener.AllowedTime);
            KeyListener.Instance.Queue(note);
            spriteRenderer.color = ClickColor;
            yield return new WaitForSecondsRealtime(KeyListener.AllowedTime);
            obj.transform.DOScale(Vector3.one * 2.25f, KeyListener.AllowedTime).SetEase(Ease.OutCubic);
            StartCoroutine(Follow(obj, note.note));
            yield return new WaitForSecondsRealtime(KeyListener.AllowedTime);
            Destroy(obj);
        }
    }
}
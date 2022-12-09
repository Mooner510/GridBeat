using System.Collections;
using DG.Tweening;
using Listener;
using Musics.Data;
using UnityEngine;
using Utils;

namespace Musics {
    public class QuadPlayer : Player {

        private static readonly Color ClickColor = new Color(0f, 0.8f, 0.8f, 0.5f);
        
        public override IEnumerator Accept(LiveNoteData note, float time) {
            yield return new WaitForSecondsRealtime(time);
            
            var noteTime = KeyListener.NoteTime / 2 - KeyListener.AllowedTime;
            
            var location = GameUtils.Locator(GameMode.Quad, note.note);
            var obj = Instantiate(beatInspector, location * 5f, Quaternion.identity);
            var spriteRenderer = obj.GetComponent<SpriteRenderer>();
            obj.transform.DOMove(location, KeyListener.NoteTime).SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(noteTime);
            KeyListener.Instance.Queue(note);
            yield return new WaitForSecondsRealtime(KeyListener.AllowedTime);
            spriteRenderer.color = ClickColor;
            yield return new WaitForSecondsRealtime(noteTime);
            obj.transform.DOScale(Vector3.one * 2.25f, noteTime).SetEase(Ease.OutCubic);
            yield return new WaitForSecondsRealtime(KeyListener.AllowedTime);
            StartCoroutine(Follow(obj, note.note, noteTime - KeyListener.AllowedTime));
            yield return new WaitForSecondsRealtime(noteTime - KeyListener.AllowedTime);
            Destroy(obj);
        }
    }
}
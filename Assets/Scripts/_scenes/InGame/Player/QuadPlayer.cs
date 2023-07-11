using System.Collections;
using _scenes.InGame.Listener;
using DG.Tweening;
using Musics;
using Musics.Data;
using UnityEngine;
using Utils;

namespace _scenes.InGame.Player {
    public class QuadPlayer : Player {

        private static readonly Color ClickColor = new(0f, 0.8f, 0.8f, 0.5f);
        
        public override IEnumerator Accept(PlayableNote note, float time) {
            // if(time > 0) yield return new WaitForSecondsRealtime(time);
            
            // var noteTime = KeyListener.NoteTime / 2 - KeyListener.AllowedTime;
            
            var location = GameUtils.Locator(GameMode.Quad, note.note.key);
            var obj = Instantiate(beatInspector, location * 5f, Quaternion.identity);
            obj.transform.DOMove(location, time).SetEase(Ease.Linear);
            yield return new WaitForSecondsRealtime(time - KeyListener.AllowedTime);
            KeyListener.Instance.Queue(note);
            yield return new WaitForSecondsRealtime(KeyListener.AllowedTime);
            obj.transform.DOScale(Vector3.one * 2.25f, KeyListener.AllowedTime).SetEase(Ease.OutCubic);
            StartCoroutine(Follow(obj, note.note.key));
            yield return new WaitForSecondsRealtime(KeyListener.AllowedTime);
            Destroy(obj);
        }
    }
}
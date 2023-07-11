using System.Collections;
using System.Collections.Generic;
using Musics;
using Score;
using UnityEngine;

namespace _scenes.InGame.Listener {
    public class QuadKeyListener : KeyListener {
        protected override void SetUp() {
            keyCodes = PlayerData.PlayerData.Instance.GetUserData().keyData.quadKey;
            Debug.Log(keyCodes);
            noteQueue = new Queue<PlayableNote>[4];
            for (var i = 0; i < 4; i++) noteQueue[i] = new Queue<PlayableNote>();
        }
        
        protected override IEnumerator Enqueue(PlayableNote data) {
            // while (NoteQueue[data.note].Count > 1) {
            //     NoteQueue[data.note].Dequeue().Click();
            //     Spawn(data, ScoreType.Miss);
            // }
            var note = data.note;
            noteQueue[note.key].Enqueue(data);
            yield return new WaitForSecondsRealtime(AllowedTime * 2);
            if (data.isClicked || (noteQueue[note.key].Count != 0 && noteQueue[note.key].Peek().isClicked)) yield break;
            data.Click();
            Spawn(data, ScoreType.Miss);
            noteQueue[note.key].Dequeue();
        }
    }
}
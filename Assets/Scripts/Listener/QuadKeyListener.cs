using System.Collections;
using System.Collections.Generic;
using Musics.Data;
using Score;
using UnityEngine;

namespace Listener {
    public class QuadKeyListener : KeyListener {
        protected override void SetUp() {
            KeyCodes = PlayerData.PlayerData.Instance.GetUserData().keyData.quadKey;
            Debug.Log(KeyCodes);
            NoteQueue = new Queue<LiveNoteData>[4];
            for (var i = 0; i < 4; i++) NoteQueue[i] = new Queue<LiveNoteData>();
        }
        
        protected override IEnumerator Enqueue(LiveNoteData data) {
            while (NoteQueue[data.note].Count > 1) {
                NoteQueue[data.note].Dequeue().Click();
                Spawn(data, ScoreType.Miss);
            }
            NoteQueue[data.note].Enqueue(data);
            yield return new WaitForSecondsRealtime(NoteTime - AllowedTime * 2);
            if (data.clicked) yield break;
            data.Click();
            Spawn(data, ScoreType.Miss);
            NoteQueue[data.note].Dequeue();
        }
    }
}
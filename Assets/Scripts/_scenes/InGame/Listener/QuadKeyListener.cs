using System.Collections;
using System.Collections.Generic;
using Musics.Data;
using Score;
using UnityEngine;

namespace InGame.Listener {
    public class QuadKeyListener : KeyListener {
        protected override void SetUp() {
            keyCodes = PlayerData.PlayerData.Instance.GetUserData().keyData.quadKey;
            Debug.Log(keyCodes);
            noteQueue = new Queue<LiveNoteData>[4];
            for (var i = 0; i < 4; i++) noteQueue[i] = new Queue<LiveNoteData>();
        }
        
        protected override IEnumerator Enqueue(LiveNoteData data) {
            // while (NoteQueue[data.note].Count > 1) {
            //     NoteQueue[data.note].Dequeue().Click();
            //     Spawn(data, ScoreType.Miss);
            // }
            noteQueue[data.note].Enqueue(data);
            yield return new WaitForSecondsRealtime(AllowedTime * 2);
            if (data.clicked || (noteQueue[data.note].Count != 0 && noteQueue[data.note].Peek().clicked)) yield break;
            data.Click();
            Spawn(data, ScoreType.Miss);
            noteQueue[data.note].Dequeue();
        }
    }
}
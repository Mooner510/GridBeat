using _scenes.InGame.Listener;
using Musics.Data;

namespace _scenes.InGame.Player {
    public class QuadTicker : Ticker {
        protected override void Tick() {
            var now = GetPlayTime() + KeyListener.NoteTime * 1000;
            var i = 0;
            do {
                var note = NewNoteManager.Pick(i).note;
                if (note.offset <= now) {
                    StartCoroutine(Player.Instance.Accept(NewNoteManager.Pop(), (now - note.offset) / 1000f + KeyListener.NoteTime));
                } else break;
            } while (!NewNoteManager.IsTop(++i));
        }
    }
}
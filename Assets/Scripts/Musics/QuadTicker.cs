using Listener;
using Musics.Data;

namespace Musics {
    public class QuadTicker : Ticker {
        protected override void Tick() {
            var now = GetPlayTime() + KeyListener.NoteTime;
            var i = 0;
            do {
                var note = NoteManager.Pick(i);
                if (note.time <= now) {
                    StartCoroutine(Player.Instance.Accept(NoteManager.Pop(), now - note.time + KeyListener.NoteTime));
                } else break;
            } while (!NoteManager.IsTop(++i));
        }
    }
}
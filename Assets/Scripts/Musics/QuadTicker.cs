using Listener;
using Musics.Data;

namespace Musics {
    public class QuadTicker : Ticker {
        protected override void Tick() {
            var now = GetPlayTime();
            var i = 0;
            do {
                var note = NoteManager.Pick(i);
                if (note.time <= now + KeyListener.NoteTime) {
                    StartCoroutine(Player.Instance.Accept(NoteManager.Pop()));
                } else break;
            } while (!NoteManager.IsTop(++i));
        }
    }
}
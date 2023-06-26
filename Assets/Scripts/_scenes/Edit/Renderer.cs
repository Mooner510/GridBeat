using System.Collections.Generic;
using Musics.Data;
using UnityEngine;

namespace _scenes.Edit {
    public class Renderer : MonoBehaviour {
        public static Renderer instance;
        private static long _renderId;

        public static long RenderId() => _renderId++;

        [SerializeField] private GameObject note;
        private readonly Queue<EditNote> _notePoolQueue = new();
        private readonly Dictionary<long, EditNote> _noteDictionary = new();

        private static EditNote GetNote() {
            var obj = instance._notePoolQueue.Count <= 0 ? instance.NewNote() : instance._notePoolQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            instance._noteDictionary.Add(obj.NoteId, obj);
            return obj;
        }

        private static void ReturnNote(long id) {
            var note = instance._noteDictionary[id];
            note.gameObject.SetActive(false);
            note.transform.SetParent(instance.transform);
            instance._notePoolQueue.Enqueue(note);
        }

        // private static void ReturnNote(EditNote note) {
        //     note.gameObject.SetActive(false);
        //     note.transform.SetParent(instance.transform);
        //     instance._notePoolQueue.Enqueue(note);
        // }
        
        private void Awake() {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        private EditNote NewNote() {
            var obj = Instantiate(note).GetComponent<EditNote>();
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(transform);
            return obj;
        }

        public void Render(MapInfo mapInfo, GameMode gameMode) {
            foreach (var o in _noteDictionary.Keys) ReturnNote(o);

            var noteMap = gameMode == GameMode.Quad ? mapInfo.noteMap.quadMap : mapInfo.noteMap.keypadMap;
            var pos = new Vector3(0, 0);
            foreach (var noteData in noteMap) {
                var obj = GetNote();
                pos.x = noteData.note;
                pos.y = noteData.time * 4;
                obj.transform.position = pos;
            }
        }
    }
}
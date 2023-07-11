using Musics.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _scenes.Main.Setting {
    public class NoteSpeed : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private bool isUp;
        
        public void OnPointerClick(PointerEventData eventData) {
            Setting.instance.NoteSpeed(isUp);
        }
    }
}
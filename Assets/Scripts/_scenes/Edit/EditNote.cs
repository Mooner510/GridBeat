using UnityEngine;

namespace _scenes.Edit {
    public class EditNote : MonoBehaviour {
        private Vector3 _dragOffset;
        public long NoteId { get; private set; }

        private void OnEnable() {
            NoteId = Renderer.RenderId();
        }

        private void OnMouseDown() {
            _dragOffset = transform.position - GetMousePosition();
        }

        private void OnMouseDrag() {
            var transformLocalPosition = transform.localPosition;
            transformLocalPosition.y = (GetMousePosition() + _dragOffset).y;
            transform.localPosition = transformLocalPosition;
        }

        private Vector3 GetMousePosition() {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            return mousePosition;
        }
    }
}
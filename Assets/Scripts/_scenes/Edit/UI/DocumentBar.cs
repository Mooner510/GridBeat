using UnityEngine;
using UnityEngine.UIElements;

namespace _scenes.Edit.UI {
    public class DocumentBar : MonoBehaviour {
        private UIDocument _document;
        
        [SerializeField] private VisualTreeAsset item;
        
        private void Start() {
            _document = GetComponent<UIDocument>();

            var listView = _document.rootVisualElement.Q<ListView>("editorList");
        }
    }
}
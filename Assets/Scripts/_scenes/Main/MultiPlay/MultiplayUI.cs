using UnityEngine;
using UnityEngine.UIElements;
using Utils;
using Button = UnityEngine.UI.Button;

namespace _scenes.Main.MultiPlay {
    public class MultiplayUI : MonoBehaviour {
        [SerializeField] private Button multiplayButton;
        [SerializeField] private GameObject multiplayUI;

        public static bool isUIEnabled;

        private VisualElement _root;

        private void Awake() {
            multiplayButton.onClick.AddListener(() => {
                _root.style.display = DisplayStyle.Flex;
                isUIEnabled = true;
            });
        }

        private void Start() {
            _root = multiplayUI.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("body");
            var connectionType = _root.Q<DropdownField>("connectionType");
            var nextButton = _root.Q<UnityEngine.UIElements.Button>("nextButton");
            var client = _root.Q<VisualElement>("clientSetting");
            var server = _root.Q<VisualElement>("serverSetting");
            connectionType.RegisterValueChangedCallback(evt => {
                var isServer = evt.newValue == "Server";
                nextButton.text = isServer ? "Server Host" : "Join Server";
                server.style.display = isServer ? DisplayStyle.Flex : DisplayStyle.None;
                client.style.display = !isServer ? DisplayStyle.Flex : DisplayStyle.None;
            });

            _root.Q<UnityEngine.UIElements.Button>("close").clicked += () => {
                _root.style.display = DisplayStyle.None;
                isUIEnabled = false;
            };
            // nextButton.clicked += () => Debug.Log("asdf");
            nextButton.clicked += Register;
            
            _root.Q<TextField>("joinCode").SetPlaceholderText("Enter Code");
            // _root.Q<UnityEngine.UIElements.Button>("nextButton").clicked += Register;
        }

        private void Update() {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            _root.style.display = DisplayStyle.None;
            isUIEnabled = false;
        }

        private void Register() {
            var connectionType = _root.Q<DropdownField>("connectionType");
            var isServer = connectionType.value == "Server";
            if (isServer) {
                var joinCode = RelayManager.CreateRelay().Result;
                var label = _root.Q<Label>("code");
                label.style.display = DisplayStyle.Flex;
                label.text = $"Room Key: {joinCode}";
            } else {
                var textField = _root.Q<TextField>("joinCode");
                var input = textField.value;
                if (input.Length == 0 || textField.style.color != Color.white) {
                    InfoTextBuilder.ShowMessage("Please enter the Join Code!", Color.red, 3, InfoTextBuilder.EaseFadeAndMoveUp);
                    return;
                }
                RelayManager.JoinRelay(input);
            }
        }
        
    }
}
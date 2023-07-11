using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Utils;
using Button = UnityEngine.UI.Button;

namespace _scenes.Main.MultiPlay {
    public class MultiplayUI : MonoBehaviour {
        [SerializeField] private Button multiplayButton;
        [SerializeField] private Text multiplayButtonText;
        [SerializeField] private GameObject multiplayUI;
        [SerializeField] private TextMeshProUGUI roomKey;

        public static bool isUIEnabled;

        private VisualElement _root;
        private UnityEngine.UIElements.Button _nextButton;

        private void Awake() {
            multiplayButton.onClick.AddListener(OpenUI);
        }

        private void CloseUI() {
            _root.style.display = DisplayStyle.None;
            isUIEnabled = false;
        }

        private void OpenUI() {
            _root.style.display = DisplayStyle.Flex;
            isUIEnabled = true;
        }

        private void Start() {
            _root = multiplayUI.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("body");
            var connectionType = _root.Q<DropdownField>("connectionType");
            _nextButton = _root.Q<UnityEngine.UIElements.Button>("nextButton");
            var client = _root.Q<VisualElement>("clientSetting");
            var server = _root.Q<VisualElement>("serverSetting");
            connectionType.RegisterValueChangedCallback(evt => {
                var isServer = evt.newValue == "Server";
                _nextButton.text = isServer ? "Server Host" : "Join Server";
                server.style.display = isServer ? DisplayStyle.Flex : DisplayStyle.None;
                client.style.display = !isServer ? DisplayStyle.Flex : DisplayStyle.None;
            });

            _root.Q<UnityEngine.UIElements.Button>("close").clicked += CloseUI;
            // nextButton.clicked += () => Debug.Log("asdf");
            _nextButton.clicked += Register;
            
            _root.Q<TextField>("joinCode").SetPlaceholderText("Enter Code");
            // _root.Q<UnityEngine.UIElements.Button>("nextButton").clicked += Register;
        }

        private void Update() {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            CloseUI();
        }

        private void Register() {
            var connectionType = _root.Q<DropdownField>("connectionType");
            var isServer = connectionType.value == "Server";
            if (isServer) {
                _nextButton.focusable = false;
                _nextButton.text = "Loading...";
                _nextButton.style.color = Color.gray;
                RelayManager.CreateRelay(joinCode => {
                    var label = _root.Q<Label>("code");
                    label.style.display = DisplayStyle.Flex;
                    roomKey.text = label.text = $"Room Key: {joinCode}";
                    _nextButton.text = "Already Hosted";
                    roomKey.gameObject.SetActive(true);
                    CloseUI();
                    multiplayButton.onClick.RemoveAllListeners();
                    multiplayButtonText.text = "Stop Host";
                    multiplayButton.onClick.AddListener(() => {
                        RelayManager.StopAllRelay();
                        roomKey.gameObject.SetActive(true);
                        multiplayButtonText.text = "Multiplay";
                        _nextButton.focusable = true;
                        _nextButton.text = "Server Host";
                        _nextButton.style.color = Color.black;
                        label.style.display = DisplayStyle.None;
                    });
                });
            } else {
                var textField = _root.Q<TextField>("joinCode");
                var input = textField.value;
                if (input.Length == 0 || textField.style.color != Color.white) {
                    InfoTextBuilder.ShowMessage("Please enter the Join Code!", Color.red, 3, InfoTextBuilder.EaseFade);
                    return;
                }
                RelayManager.JoinRelay(input);
            }
        }
        
    }
}
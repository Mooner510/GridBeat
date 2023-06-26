using UnityEngine;
using UnityEngine.UIElements;

namespace Utils {
    public static class UIUtils {
        public static void SetPlaceholderText(this TextField textField, string placeholder) {
            var placeholderClass = TextField.ussClassName + "__placeholder";

            OnFocusOut();
            textField.RegisterCallback<FocusInEvent>(_ => OnFocusIn());
            textField.RegisterCallback<FocusOutEvent>(_ => OnFocusOut());

            void OnFocusIn() {
                if (!textField.ClassListContains(placeholderClass)) return;
                textField.value = string.Empty;
                textField.style.color = Color.white;
                textField.RemoveFromClassList(placeholderClass);
            }

            void OnFocusOut() {
                if (!string.IsNullOrEmpty(textField.text)) return;
                textField.SetValueWithoutNotify(placeholder);
                textField.style.color = Color.gray;
                textField.AddToClassList(placeholderClass);
            }
        }
    }
}
using System;
using UnityEngine;

namespace PlayerData.Entity {
    [Serializable]
    public class UserData {
        public KeyData keyData = new();
    }

    [Serializable]
    public class KeyData {
        public KeyCode[] keypadKey = {
            KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6,
            KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3
        };
        public KeyCode[] quadKey = {
            KeyCode.F, KeyCode.J, KeyCode.R, KeyCode.I
        };
        public KeyCode[] quadKey2 = {
            KeyCode.Keypad5, KeyCode.Keypad6, KeyCode.Keypad7, KeyCode.KeypadMinus
        };
    }
}
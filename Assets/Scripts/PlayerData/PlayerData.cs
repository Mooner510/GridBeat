using PlayerData.Entity;
using Utils;

namespace PlayerData {
    public class PlayerData : SingleTon<PlayerData> {
        private UserData _userData;

        public PlayerData() => Load();

        private void Load() {
            // _userData = Json.LoadJsonFile<UserData>("Assets/Data/userData") ?? new UserData();
            _userData = new UserData();
        }

        public UserData GetUserData() => _userData;
    }
}
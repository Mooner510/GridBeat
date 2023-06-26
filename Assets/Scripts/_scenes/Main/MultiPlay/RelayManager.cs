using System.Threading.Tasks;
using JetBrains.Annotations;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace _scenes.Main.MultiPlay {
    public class RelayManager : MonoBehaviour {
        private async void Awake() {
            if (UnityServices.State == ServicesInitializationState.Initialized) return;
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () => {
                Debug.Log($"Signed in with ID: {AuthenticationService.Instance.PlayerId}");
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        [ItemCanBeNull]
        public static async Task<string> CreateRelay() {
            try {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log(joinCode);

                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartHost();

                return joinCode;
            } catch (RelayServiceException e) {
                InfoTextBuilder.ShowMessage(e.Message, Color.red, 4, InfoTextBuilder.EaseFadeAndMoveUp);
            }

            return null;
        }

        public static async void JoinRelay(string joinCode) {
            try {
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();
            } catch (RelayServiceException e) {
                InfoTextBuilder.ShowMessage(e.Message, Color.red, 4, InfoTextBuilder.EaseFadeAndMoveUp);
            }
        }
    }
}
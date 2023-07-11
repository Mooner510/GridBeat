using System;
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
        public static string HostKey { get; private set; }
        
        private async void Awake() {
            if (UnityServices.State == ServicesInitializationState.Initialized) return;
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () => {
                Debug.Log($"Signed in with ID: {AuthenticationService.Instance.PlayerId}");
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void OnApplicationQuit() => StopAllRelay();

        private void OnDestroy() => StopAllRelay();

        public static void StopAllRelay() {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Stop Networking");
        }

        public static async void CreateRelay(Action<string> after) {
            try {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log(joinCode);

                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartHost();

                HostKey = joinCode;
                after.Invoke(joinCode);
            } catch (RelayServiceException e) {
                InfoTextBuilder.ShowMessage(e.Message, Color.red, 4, InfoTextBuilder.EaseFade);
            }
        }

        public static async void JoinRelay(string joinCode) {
            try {
                JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();
            } catch (RelayServiceException e) {
                InfoTextBuilder.ShowMessage(e.Message, Color.red, 4, InfoTextBuilder.EaseFade);
            }
        }
    }
}
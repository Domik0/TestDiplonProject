using System.Linq;
using System.Threading;
using Assets.Scripts.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StarterAssets
{
    /// <summary>
    /// Класс отвечающий за работу Лобби
    /// </summary>

    public class LobbyUI : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private LobbyPlayerCard[] lobbyPlayerCards;
        [SerializeField] private GameObject errorPanel;
        [SerializeField] private Button startGameButton;
        [SerializeField] private TextMeshProUGUI joinCodeTextMeshProUgui;
        [SerializeField] private NetworkVariable<bool> hostExists;
        [SerializeField] private NetworkVariable<NetworkString> joinCode;
        private NetworkList<LobbyPlayerState> lobbyPlayers;

        private void Awake()
        {
            lobbyPlayers = new NetworkList<LobbyPlayerState>();
        }

        public void Update()
        {
            if (IsHost&&hostExists.Value&& lobbyPlayers.Count==1)
            {
                Debug.Log(lobbyPlayers.Count);
               GameNetPortal.Instance.RequestDisconnect();
            }
        }

        public override void OnNetworkSpawn()
        {
            
            if (IsClient)
            {
                lobbyPlayers.OnListChanged += HandleLobbyPlayersStateChanged;
            }

            if (IsServer)
            {
                startGameButton.gameObject.SetActive(true);
                joinCode.Value = RelayManager.Instance.joinCode;
                NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    HandleClientConnected(client.ClientId);
                }
            }
            joinCodeTextMeshProUgui.text = $"JOIN CODE: {joinCode.Value}";
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            lobbyPlayers.OnListChanged -= HandleLobbyPlayersStateChanged;

            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
            }
        }

        private bool IsEveryoneReady()
        {
            if (lobbyPlayers.Count < 2)
            {
                return false;
            }

            foreach (var player in lobbyPlayers)
            {
                if (!player.IsReady)
                {
                    return false;
                }
            }

            return true;
        }



        /// <summary>
        /// Метод который срабатывает при подключения клиента
        /// </summary>
        /// <param name="clientId">Id клиента, который подключается</param>

        private void HandleClientConnected(ulong clientId)
        {
            var playerData = ServerGameNetPortal.Instance.GetPlayerData(clientId);
            if (!playerData.HasValue) { return; }

            lobbyPlayers.Add(new LobbyPlayerState(
                clientId,
                playerData.Value.PlayerName,
                false
            ));
        }


        /// <summary>
        /// Срабатывает при отключения клиента
        /// </summary>
        /// <param name="clientId"></param>
        private void HandleClientDisconnect(ulong clientId)
        {
           
            for (int i = 0; i < lobbyPlayers.Count; i++)
            {
                if (lobbyPlayers[i].ClientId == clientId)
                {
                    lobbyPlayers.RemoveAt(i);
                    break;
                }
            }
          
        }

        /// <summary>
        /// Команда для сменны готовности игрока
        /// </summary>
        /// <param name="serverRpcParams"></param>
        [ServerRpc(RequireOwnership = false)]
        private void ToggleReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            for (int i = 0; i < lobbyPlayers.Count; i++)
            {
                if (lobbyPlayers[i].ClientId == serverRpcParams.Receive.SenderClientId)
                {
                    lobbyPlayers[i] = new LobbyPlayerState(
                        lobbyPlayers[i].ClientId,
                        lobbyPlayers[i].PlayerName,
                        !lobbyPlayers[i].IsReady
                    );
                }
            }
        }


        [ClientRpc]
        private void DisconectCallbackClientRpc()
        {
            if (!IsHost)
            {
                GameNetPortal.Instance.RequestDisconnect();
            }
            hostExists.Value = true;
            Debug.Log(hostExists.Value);
        }

        [ServerRpc]
        private void DisconectAllClientServerRpc()
        {
            DisconectCallbackClientRpc();
        }

       


        [ServerRpc(RequireOwnership = false)]
        private void StartGameServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (serverRpcParams.Receive.SenderClientId != NetworkManager.Singleton.LocalClientId) { return; }

            if (!IsEveryoneReady()) { return; }
          
            ServerGameNetPortal.Instance.StartGame();
        }

        public void OnLeaveClicked()
        {
          
            if (IsHost)
            {
                DisconectAllClientServerRpc();
            }
            else
            {
                GameNetPortal.Instance.RequestDisconnect();
            }
            

        }

        public void OnReadyClicked()
        {
            ToggleReadyServerRpc();
        
        }

        public void OnStartGameClicked()
        {
            StartGameServerRpc();
        }

        /// <summary>
        /// Метод окопирует JOIN CODE в буфер обмена
        /// </summary>
        public void OnCopyJoinCode()
        {
            GUIUtility.systemCopyBuffer = joinCode.Value;
        }

        /// <summary>
        /// Метод обновляет состояние мест игроков
        /// </summary>
        /// <param name="lobbyState">Список мест</param>
        private void HandleLobbyPlayersStateChanged(NetworkListEvent<LobbyPlayerState> lobbyState)
        {
            for (int i = 0; i < lobbyPlayerCards.Length; i++)
            {
                if (lobbyPlayers.Count > i)
                {
                    lobbyPlayerCards[i].UpdateDisplay(lobbyPlayers[i]);
                }
                else
                {
                    lobbyPlayerCards[i].DisableDisplay();
                }
            }

            if(IsHost)
            {
                startGameButton.interactable = IsEveryoneReady();
            }
        }
    }
}

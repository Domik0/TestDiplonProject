using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Networking;
using Unity.Netcode;

namespace Assets.Scripts
{
    public class PlayersManager : NetworkSingleton<PlayersManager>
    {
        NetworkVariable<int> playersInGame = new NetworkVariable<int>();

        public int PlayersInGame
        {
            get
            {
                return playersInGame.Value;
            }
        }

        void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                if (IsServer)
                    playersInGame.Value++;
            };

            NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
            {
                if (IsServer)
                    playersInGame.Value--;
            };
        }
    }
}

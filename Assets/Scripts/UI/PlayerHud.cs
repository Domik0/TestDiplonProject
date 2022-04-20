using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class PlayerHud:NetworkBehaviour
    {
        [SerializeField]
        private NetworkVariable<NetworkString> playerNetworkName = new NetworkVariable<NetworkString>();

        private bool _overlaySet = false;

        private void Start()
        {
            if (IsClient& IsOwner)
            {
                string nickName= PlayerPrefs.GetString("PlayerName");
                SetNicknameServerRpc(nickName);
            }
          
        }

        [ServerRpc]
        private void SetNicknameServerRpc(string name)
        {
            playerNetworkName.Value = name;
        }



        public void SetOverlay()
        {
            var localPlayerOverlay = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var textBox in localPlayerOverlay)
            {
                textBox.text = playerNetworkName.Value;
            }
        }

        public void Update()
        {
            if (!_overlaySet && !string.IsNullOrEmpty(playerNetworkName.Value))
            {
                SetOverlay();
                _overlaySet = true;
            }
        }
    }
}

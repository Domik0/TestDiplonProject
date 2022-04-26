﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Networking;
using StarterAssets;
using TMPro;
using Unity.Netcode;

namespace Assets.Scripts.UI
{
    class UIStatList : NetworkSingleton<UIManager>
    {

        public TextMeshProUGUI playerOneText;
        public TextMeshProUGUI playerSecondText;
        public TextMeshProUGUI playerThirdText;
        public TextMeshProUGUI playerFourthText;

        private List<ThirdPersonController> personsList = new List<ThirdPersonController>();

        void Start()
        {
            if (IsServer)
            {
                GetPersonControllers();
                GenerateStatPersonServerRpc();
            }
        }

        private void GetPersonControllers()
        {
            foreach (var person in NetworkManager.Singleton.ConnectedClients)
            {
                personsList.Add(person.Value.PlayerObject.GetComponent<ThirdPersonController>());
            }

            personsList.OrderBy(p => p.timeTag);
        }

        [ServerRpc]
        private void GenerateStatPersonServerRpc()
        {
            if(personsList[0] != null)
            {
                playerOneText.gameObject.SetActive(true);
                playerOneText.text = "1. " + "player" + personsList[0].timeTag.ToString(@"mm\:ss");
            }
            if (personsList[1] != null)
            {
                playerOneText.gameObject.SetActive(true);
                playerOneText.text = "2. " + "player" + personsList[1].timeTag.ToString(@"mm\:ss");
            }
            if (personsList[2] != null)
            {
                playerOneText.gameObject.SetActive(true);
                playerOneText.text = "3. " + "player" + personsList[2].timeTag.ToString(@"mm\:ss");
            }
            if (personsList[3] != null)
            {
                playerOneText.gameObject.SetActive(true);
                playerOneText.text = "4. " + "player" + personsList[3].timeTag.ToString(@"mm\:ss");
            }

        }

        public void ExitClick()
        {
            GameNetPortal.Instance.RequestDisconnect();
        }
    }
}

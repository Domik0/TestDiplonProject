using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Networking;
using StarterAssets;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class UIStatList : NetworkSingleton<UIStatList>
    {

        public List<NetworkObject> ListGameObjects;
        public List<TextMeshProUGUI> listText;

        private List<ThirdPersonController> personsList = new List<ThirdPersonController>();

        private  NetworkList<PlayerStat> cooList = new NetworkList<PlayerStat>();


        void Start()
        {

            if (IsServer)
            {
                DestroyAllObjectServerRpc();
                foreach (var person in NetworkManager.Singleton.ConnectedClients)
                {
                    personsList.Add(person.Value.PlayerObject.GetComponent<ThirdPersonController>());
                }

                personsList=  personsList.OrderBy(p => p.timeTag.Value).ToList();
                for (int i = 0; i < personsList.Count; i++)
                {
                    if (personsList[i] != null)
                    {
                        cooList.Add(new PlayerStat()
                        {
                            PlayerNum = i + 1,
                            PlayerName = new FixedString32Bytes(personsList[i].nickName.Value),
                            TimeTag = personsList[i].timeTag.Value
                        });
                    }
                }
            }
            GenerateStatPerson();
        }



        [ServerRpc]
        private void DestroyAllObjectServerRpc()
        {
            var list = GameObject.FindGameObjectsWithTag("Player").Union(GameObject.FindGameObjectsWithTag("Chest"));
            foreach (var item in list)
            {
                Destroy(item);
            }
        }


        private void GenerateStatPerson()
        {
            for (int i = 0; i < cooList.Count; i++)
            {

                listText[i].gameObject.SetActive(true);
                listText[i].text = $"{cooList[i].PlayerNum} {cooList[i].PlayerName.Value} {cooList[i].TimeTag.ToString(@"mm\:ss")}";
            }
            


        }

        public void ExitClick()
        {
            GameNetPortal.Instance.RequestDisconnect();
        }
    }
}

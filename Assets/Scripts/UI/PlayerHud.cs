using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Networking;
using Cinemachine;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.UI
{
    class PlayerHud:NetworkBehaviour
    {
        [SerializeField]
        private NetworkVariable<NetworkString> playerNetworkName = new NetworkVariable<NetworkString>();

        [SerializeField] private Transform mainCameraTransform;
        [SerializeField] public GameObject billbord;


        private bool _overlaySet = false;

        private void Awake()
        {
            mainCameraTransform = FindObjectOfType<Camera>().transform;
        }

        private void Start()
        {
            
            if (IsClient & IsOwner)
            {
                string nickName = PlayerPrefs.GetString("PlayerName");
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
            var localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
            localPlayerOverlay.text = playerNetworkName.Value;
            
        }

        public void Update()
        {
            if (!_overlaySet && !string.IsNullOrEmpty(playerNetworkName.Value))
            {
                SetOverlay();
                _overlaySet = true;
            }
           
        }

        private void LateUpdate()
        {
            if (mainCameraTransform != null)
            {
                //transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation* Vector3.up
                billbord.transform.LookAt(mainCameraTransform);
                billbord.transform.Rotate(Vector3.up * 180);
            }
        }


      
    }
}

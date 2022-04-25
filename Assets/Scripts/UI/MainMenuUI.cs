using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;

namespace StarterAssets
{
    /// <summary>
    ///  Класс MainMenuUI
    ///  Скрипт главного меню игры
    /// </summary>
    /// 
    public class MainMenuUI : MonoBehaviour
    {
        [Header("References")]

        [SerializeField]
        private TMP_InputField displayNameInputField;

        [SerializeField]
        private TMP_InputField displayPortInputField;

        [SerializeField]
        private GameObject errorPanel;

        [SerializeField] 
        private TextMeshProUGUI textError;

        [SerializeField]
        private GameObject goUNetTransport;

        private UNetTransport uNetTransport;


        private void Start()
        {
            uNetTransport = goUNetTransport.GetComponent<UNetTransport>();
            PlayerPrefs.GetString("PlayerName");
            displayPortInputField.text = uNetTransport.ConnectPort.ToString();
        }

        public void OnHostClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            if (CheckNickname())
            {
                try
                {
                    GameNetPortal.Instance.StartHost();
                }
                catch (Exception e)
                {
                    textError.text = "Change port";

                    errorPanel.SetActive(true);
                    throw;
                }
                
            }
            
        }

        public void OnClientClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            if (CheckNickname())
            {
                ClientGameNetPortal.Instance.StartClient();
            }
        }

        private bool CheckNickname()
        {
            if (displayNameInputField.text == "")
            {
                textError.text = "Enter nickname";
                errorPanel.SetActive(true);
                return false;
            }
            return true;
        }
    }
}


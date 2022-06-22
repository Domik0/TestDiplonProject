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
        private TMP_InputField displayJoinInputField;
        [SerializeField]
        private GameObject errorPanel;
        [SerializeField]
        private TextMeshProUGUI textError;

        private void Start()
        {
            PlayerPrefs.GetString("PlayerName");
        }

        public async void OnHostClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            if (CheckNickname())
            {
                if (RelayManager.Instance.IsRelayEnabled)
                {
                    await RelayManager.Instance.SetupRelay();
                }
                GameNetPortal.Instance.StartHost();
            }
        }

        public async void OnClientClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            if (CheckNickname())
            {
                if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(displayJoinInputField.text))
                {
                    try
                    {
                        await RelayManager.Instance.JoinRelay(displayJoinInputField.text);
                        ClientGameNetPortal.Instance.StartClient(displayJoinInputField.text);
                    }
                    catch (Exception e)
                    {
                        errorPanel.SetActive(true);
                        textError.text = "Join code don't work";
                    }
                }
                else
                {
                    errorPanel.SetActive(true);
                    textError.text = "Unable start client";
                }
            }
        }

        private bool CheckNickname()
        {
            if (displayNameInputField.text == "")
            {
                errorPanel.SetActive(true);
                textError.text = "Enter Nickname";
                return false;
            }
            return true;
        }
    }
}


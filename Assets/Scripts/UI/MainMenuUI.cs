using TMPro;
using Unity.Netcode;
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
        private GameObject errorPanel;

        [SerializeField] 
        private TextMeshProUGUI textError;


        private void Start()
        {
            PlayerPrefs.GetString("PlayerName");
        }

        public void OnHostClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            if (CheckNickname())
            {
               
                    GameNetPortal.Instance.StartHost();
                
            }
            
        }

        public void OnClientClicked()
        {
            PlayerPrefs.SetString("PlayerName", displayNameInputField.text);
            if (CheckNickname())
            {
                if (CheckHost())
                {
                    
                    ClientGameNetPortal.Instance.StartClient();
                }
               
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

        private bool CheckHost()
        {
            if (NetworkManager.Singleton.ConnectedHostname==null)
            {
                textError.text = "Game is not started";
                errorPanel.SetActive(true);
                return false;
            }
            return true;
        }

    }
}


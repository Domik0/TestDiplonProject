using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Assets.Scripts.Networking;
using StarterAssets;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : NetworkSingleton<UIManager>
{
    public NetworkVariable<bool> timerActive = new NetworkVariable<bool>();
    private NetworkVariable<TimeSpan> currentTime = new NetworkVariable<TimeSpan>();
    public int startMinutes;
    public TextMeshProUGUI currentTimeText;
    public GameObject Buttons;

    void Awake()
    {
        NetworkManager.SceneManager.OnLoadComplete += SceneManager_OnLoadComplete;
    }

    private void SceneManager_OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (sceneName == "Scene_Main")
        {
            StartTimeServerRpc();
        }
    }

    void Update()
    {
        if (IsServer)
        {
            if (timerActive.Value)
            {
                currentTime.Value -= TimeSpan.FromSeconds(Time.deltaTime);
                if (currentTime.Value <= TimeSpan.Zero)
                {
                    StopTime();
                    SceneLoaderWrapper.Instance.LoadScene("Scene_EndGame", true);
                }
            }
        }
        currentTimeText.text = currentTime.Value.ToString(@"mm\:ss");
    }



    [ServerRpc(RequireOwnership = false)]
    void StartTimeServerRpc()
    {
        timerActive.Value = true;
        currentTime.Value = TimeSpan.FromSeconds(startMinutes * 60);
        Buttons.SetActive(true);
    }


    public void StopTime()
    {
        foreach (var person in NetworkManager.Singleton.ConnectedClients)
        {
            person.Value.PlayerObject.GetComponent<ThirdPersonController>().isTag.Value = false;
        }
        timerActive.Value = false;
        SpawnManager.loudingCount.Value = 0;
       
    }

    public void ExitGame()
    {
        if (IsServer)
        {
            StopTime();
            SceneLoaderWrapper.Instance.LoadScene("Scene_EndGame", true);
        }
        else
        {
            GameNetPortal.Instance.RequestDisconnect();
        }
    }
}
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

public class UIManager : NetworkSingleton<UIManager>
{
    public NetworkVariable<bool> timerActive = new NetworkVariable<bool>();
    private NetworkVariable<TimeSpan> currentTime = new NetworkVariable<TimeSpan>();
    private NetworkVariable<TimeSpan> startTime = new NetworkVariable<TimeSpan>();
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
                UpdateTimeGameServerRpc();
                if (currentTime.Value <= TimeSpan.Zero)
                {
                    StopTimeServerRpc();
                    SceneLoaderWrapper.Instance.LoadScene("Scene_EndGame", true);
                }
            }
        }
        currentTimeText.text = currentTime.Value.ToString(@"mm\:ss");
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateTimeGameServerRpc()
    {
        currentTime.Value -= TimeSpan.FromSeconds(Time.deltaTime);
        startTime.Value += TimeSpan.FromSeconds(Time.deltaTime);
    }

    [ServerRpc(RequireOwnership = false)]
    void StartTimeServerRpc()
    {
        timerActive.Value = true;
        currentTime.Value = TimeSpan.FromSeconds(startMinutes * 60);
        startTime.Value = TimeSpan.Zero;
        Buttons.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StopTimeServerRpc()
    {
        timerActive.Value = false;
        SpawnManager.loudingCount.Value = 0;
    }

    public void ExitGame()
    {
        if (IsServer)
        {
            var player = NetworkManager.SpawnManager.GetPlayerNetworkObject(NetworkManager.LocalClientId)
                .GetComponent<ThirdPersonController>().isTag;
            if (player.Value)
            {
                player.Value = false;
            }
            StopTimeServerRpc();

            SceneLoaderWrapper.Instance.LoadScene("Scene_EndGame", true);
        }
        else
        {
            GameNetPortal.Instance.RequestDisconnect();
        }
    }
}
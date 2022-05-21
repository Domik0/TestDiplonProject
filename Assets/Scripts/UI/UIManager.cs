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
    public NetworkVariable<bool> timerActive=new NetworkVariable<bool>();
    private NetworkVariable<TimeSpan> currentTime = new NetworkVariable<TimeSpan>();
    private NetworkVariable<TimeSpan> startTime = new NetworkVariable<TimeSpan>();
    public int startMinutes;
    public int loadingSeconds;
    public TextMeshProUGUI currentTimeText;
    public GameObject LoadindScene;
    public GameObject Buttons;

    void Start()
    {
        //NetworkManager.SceneManager.OnLoadComplete += SceneManager_OnLoadComplete;
        //NetworkManager.SceneManager.OnSynchronizeComplete += SceneManager_OnSynchronizeComplete; ;

    }

    private void SceneManager_OnSynchronizeComplete(ulong clientId)
    {
        Buttons.SetActive(false);
    }

    private void SceneManager_OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        if (sceneName == "Scene_Main")
        {
            timerActive.Value = true;
            currentTime.Value = TimeSpan.FromSeconds(startMinutes * 60 + loadingSeconds);
            startTime.Value = TimeSpan.Zero;
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


    

    [ServerRpc]
    void UpdateTimeGameServerRpc()
    {
        currentTime.Value -= TimeSpan.FromSeconds(Time.deltaTime);
        startTime.Value += TimeSpan.FromSeconds(Time.deltaTime);
    }

    //[ServerRpc]
    //void UpdateCurentTimeServerRpc()
    //{
    //    currentTime.Value = TimeSpan.FromSeconds(startMinutes * 60 + loadingSeconds);
    //    startTime.Value = TimeSpan.Zero;
    //}

    [ServerRpc]
    public void StartTimerServerRpc()
    {
        
        
    }

    [ServerRpc]
    public void StopTimeServerRpc()
    {
        timerActive.Value = false;
    }
}
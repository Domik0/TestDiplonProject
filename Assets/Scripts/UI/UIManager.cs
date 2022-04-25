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
    private bool timerActive = true;
    private NetworkVariable<TimeSpan> currentTime = new NetworkVariable<TimeSpan>();
    private NetworkVariable<TimeSpan> startTime = new NetworkVariable<TimeSpan>();
    public int startMinutes;
    public int loadingSeconds;
    public TextMeshProUGUI currentTimeText;
    public GameObject LoadindScene;
    public GameObject Buttons;

    void Start()
    {
        if (IsServer)
        {
            UpdateCurentTimeServerRpc();
        }
    }

    void Update()
    {
        if (IsServer)
        {
            if (timerActive)
            {
                UpdateTimeGameServerRpc();
                if (currentTime.Value <= TimeSpan.Zero)
                {
                    timerActive = false;
                    //GameNetPortal.Instance.RequestDisconnect();
                    NetworkManager.Singleton.SceneManager.LoadScene("Scene_EndGame", LoadSceneMode.Single);
                }
            }
        }

        if (startTime.Value >= TimeSpan.FromSeconds(loadingSeconds))
        {
            LoadindScene.SetActive(false);
            Buttons.SetActive(true);
        }
        
        currentTimeText.text = currentTime.Value.ToString(@"mm\:ss");
    }

    [ServerRpc]
    void UpdateTimeGameServerRpc()
    {
        currentTime.Value -= TimeSpan.FromSeconds(Time.deltaTime);
        startTime.Value += TimeSpan.FromSeconds(Time.deltaTime);
    }

    [ServerRpc]
    void UpdateCurentTimeServerRpc()
    {
        currentTime.Value = TimeSpan.FromSeconds(startMinutes * 60 + loadingSeconds);
        startTime.Value = TimeSpan.Zero;
    }


    public void StartTimer()
    {
        timerActive = true;
    }

    public void StopTimer()
    {
        timerActive = false;
    }
}
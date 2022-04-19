using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Assets.Scripts.Networking;
using StarterAssets;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkSingleton<UIManager>
{
    private bool timerActive = true;
    private NetworkVariable<float> currentTime=new NetworkVariable<float>();
    private NetworkVariable<float> startTime= new NetworkVariable<float>();
    public int startMinutes;
    public int loadingMinutes;
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
        if(IsServer)
        {
            if (timerActive)
            {
                UpdateTimeGameServerRpc();
                if (currentTime.Value <= 0)
                {
                    timerActive = false;
                    GameNetPortal.Instance.RequestDisconnect();
                }
            }
        }

        if (startTime.Value >= loadingMinutes)
        {
            LoadindScene.SetActive(false);
            Buttons.SetActive(true);
        }

        TimeSpan time = TimeSpan.FromSeconds(currentTime.Value);
        currentTimeText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString();
    }

    [ServerRpc]
    void UpdateTimeGameServerRpc()
    {
        currentTime.Value -= Time.deltaTime;
        startTime.Value += Time.deltaTime;
    }

    [ServerRpc]
    void UpdateCurentTimeServerRpc()
    {
        currentTime.Value = startMinutes * 60 + loadingMinutes;
        startTime.Value = 0;
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
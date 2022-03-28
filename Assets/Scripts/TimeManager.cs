using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Assets.Scripts.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : NetworkSingleton<TimeManager>
{
    private bool timerActive = true;
    private NetworkVariable<float> currentTime;
    public int startMinutes;
    public TextMeshProUGUI currentTimeText;

    void Start()
    {
        currentTime.Value = startMinutes * 60;
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
                }
            }
        }

        TimeSpan time = TimeSpan.FromSeconds(currentTime.Value);
        currentTimeText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString();
    }

    [ServerRpc]
    void UpdateTimeGameServerRpc()
    {
        currentTime.Value -= Time.deltaTime;
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
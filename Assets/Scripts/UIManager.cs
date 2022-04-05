using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using Assets.Scripts;
using Assets.Scripts.Networking;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkSingleton<UIManager>
{
    private bool timerActive = true;
    private NetworkVariable<float> currentTime;
    public int startMinutes;
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI playersInGameText;

    void Start()
    {
        currentTime.Value = startMinutes * 60;
    }

    void Update()
    {
        CheckPlayersInGame();
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

    private void CheckPlayersInGame()
    {
        playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}/4";
        if(PlayersManager.Instance.PlayersInGame == 4 && timerActive == false)
            timerActive = true;
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
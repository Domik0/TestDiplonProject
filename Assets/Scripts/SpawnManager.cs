using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] NetworkObject PlayerPrefab;
    [SerializeField] private Vector2 defaultInitialPositionOnPlane = new Vector2(-4, 4);

    private NetworkList<LobbyPlayerState> listNickname = new NetworkList<LobbyPlayerState>();
    private static NetworkVariable<int> rndTag = new NetworkVariable<int>();
    public static NetworkVariable<int> loudingCount = new NetworkVariable<int>();

    private void Start()
    {
        if (IsServer)
        {
            rndTag.Value = Random.Range(1, ServerGameNetPortal.Instance.clientData.Count);
            foreach (var item in ServerGameNetPortal.Instance.clientData.Values)
            {
                listNickname.Add(new LobbyPlayerState()
                {
                    ClientId = item.ClientId,
                    PlayerName = item.PlayerName
                });
            }
        }
        AddConnectPlayerServerRpc();
        foreach (var item in listNickname)
        {
            if (item.ClientId == NetworkManager.LocalClientId)
            {
                SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId, item.PlayerName.Value);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddConnectPlayerServerRpc()
    {
        loudingCount.Value++;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong localClientId, string nick)
    {
        Vector3 positon = new Vector3(Random.Range(defaultInitialPositionOnPlane.x, defaultInitialPositionOnPlane.y), 0,
                    Random.Range(0, 2));
        var go = Instantiate(PlayerPrefab, positon, Quaternion.identity);
        var controller = go.GetComponent<ThirdPersonController>();
        controller.nickName.Value = nick;
        if (loudingCount.Value == rndTag.Value)
        {
            controller.isTag.Value = true;
        }
        go.SpawnAsPlayerObject(localClientId);
    }
}

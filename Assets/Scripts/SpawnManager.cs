using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] NetworkObject PlayerPrefab;
    [SerializeField] NetworkObject ChestPrefab;

    private NetworkList<int> listSpawnPlayer = new NetworkList<int>();
    private List<int> listSpawnChest = new List<int>();
    private static NetworkVariable<bool> isTagSpawned = new NetworkVariable<bool>();

    private static NetworkVariable<int> rndTag = new NetworkVariable<int>();
    private static NetworkVariable<int> loudingCount = new NetworkVariable<int>();

    private void Start()
    {
        
        if (IsServer && rndTag.Value == 0)
        {
            rndTag.Value = Random.Range(1, ServerGameNetPortal.Instance.clientData.Count);
        }
        AddConnectPlayerServerRpc();
        //int rndSpawnPointId;
        //do
        //{
        //    rndSpawnPointId = Random.Range(0, 4);
        //}
        //while (listSpawnPlayer.Contains(rndSpawnPointId));
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            //SpawnBonusServerRpc(rndSpawnPointId);
    }


    [ServerRpc(RequireOwnership = false)]
    private void AddConnectPlayerServerRpc()
    {
        loudingCount.Value++;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong localClientId)
    {
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        var go = Instantiate(PlayerPrefab, spawnPos, spawnRot );
        if (loudingCount.Value == rndTag.Value)
        {
            go.GetComponent<ThirdPersonController>().isTag.Value = true;
        }
        go.SpawnAsPlayerObject(localClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBonusServerRpc(int rndSpawnPointId)
    {
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        switch (rndSpawnPointId)
        {
            case 0:
                spawnPos = new Vector3(0.35574f, 4f, -3.69113f);
                spawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 1:
                spawnPos = new Vector3(-12.57008f, 1.538f, 28.89237f);
                spawnRot = Quaternion.Euler(0f, 90f, 0f);
                break;
            case 2:
                spawnPos = new Vector3(-22.00712f, 1.201143f, 30.67501f);
                spawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 3:
                spawnPos = new Vector3(18.21362f, 2.960016f, 19.52146f);
                spawnRot = Quaternion.Euler(0f, 180f, 0f);
                break;
        }
        var go = Instantiate(ChestPrefab, spawnPos, spawnRot);
        go.Spawn();
    }
}

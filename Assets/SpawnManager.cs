using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager:NetworkBehaviour
{
    [SerializeField] NetworkObject PlayerPrefab;
    [SerializeField] NetworkObject ChestPrefab;

    private List<int> listSpawnPlayer = new List<int>();
    private List<int> listSpawnChest = new List<int>();

    private void Start()
    {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        SpawnBonusServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong localClientId)
    {
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        int rndSpawnPointId = Random.Range(1, 4);
        while (listSpawnPlayer.Contains(rndSpawnPointId))
        {
            rndSpawnPointId = Random.Range(1, 4);
        }

        switch (rndSpawnPointId)
        {
            case 0:
                spawnPos = new Vector3(0f, 0f, 0f);
                spawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 1:
                spawnPos = new Vector3(2f, 0f, 0f);
                spawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 2:
                spawnPos = new Vector3(4f, 0f, 0f);
                spawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;
            case 3:
                spawnPos = new Vector3(6f, 0f, 0f);
                spawnRot = Quaternion.Euler(0f, 0f, 0f);
                break;
        }
        var go = Instantiate(PlayerPrefab, spawnPos, spawnRot);
        go.SpawnAsPlayerObject(localClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBonusServerRpc()
    {
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        int rndSpawnPointId = Random.Range(1, 4);
        while (listSpawnPlayer.Contains(rndSpawnPointId))
        {
            rndSpawnPointId = Random.Range(1, 4);
        }

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

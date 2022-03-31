using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager:NetworkBehaviour
{
    [SerializeField] NetworkObject PlayerPrefab;

    private void Start()
    {
        SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong localClientId)
    {
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        switch (localClientId)
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
    private void SpawnBonusServerRpc(ulong localClientId)
    {
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;

        switch (localClientId)
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
        //var go = Instantiate(PlayerPrefab, spawnPos, spawnRot);
        //go.SpawnAsPlayerObject(localClientId);
    }
}

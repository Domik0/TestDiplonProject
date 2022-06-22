using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;

public class Slowdown : NetworkBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        var th = collision.GetComponent<ThirdPersonController>();
        if (th != null)
        {
            SlowServerRpc(th.OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SlowServerRpc(ulong clientId)
    {
        NetworkManager.Singleton.ConnectedClients[clientId]
                .PlayerObject.GetComponent<ThirdPersonController>().SlowMove();
    }
}

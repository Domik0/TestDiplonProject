using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroyEffect : NetworkBehaviour
{
    // Start is called before the first frame update
    ParticleSystem bonus;

    public override void OnNetworkSpawn()
    {
        bonus = GetComponent<ParticleSystem>();
        DestroyBonusServerRpc(bonus.duration);
    }


    [ServerRpc(RequireOwnership =false)]
    void DestroyBonusServerRpc(float duration)
    {
        Destroy(gameObject,duration);
    }
}

using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Slowdown : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        var th = collision.GetComponent<ThirdPersonController>();
        if (th != null)
        {
            th.SlowMove();
        }
    }
}

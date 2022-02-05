using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Slowdown : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Tag")
        {
            collision.gameObject.GetComponent<ThirdPersonController>().MoveSpeed -= 1;
            collision.gameObject.GetComponent<ThirdPersonController>().SprintSpeed -= 1;
            Debug.Log("Замедляемся");
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Tag")
        {
            collision.gameObject.GetComponent<ThirdPersonController>().MoveSpeed += 1;
            collision.gameObject.GetComponent<ThirdPersonController>().SprintSpeed += 1;
            Debug.Log("Не замедляемься");
        }
    }
}

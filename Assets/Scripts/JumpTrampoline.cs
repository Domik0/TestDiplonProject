using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JumpTrampoline : MonoBehaviour
{
    public float bounce = 10;
    private float gravity = -9.81f;
    private float velocity;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var controller = collision.gameObject.GetComponent<CharacterController>();
            velocity += gravity * Time.deltaTime;
            controller.transform.Translate(new Vector3(0, velocity, 0) * Time.deltaTime);
        }
    }
}

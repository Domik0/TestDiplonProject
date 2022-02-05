using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour
{
    public float delay = 3f;
    public bool isThrow;

    private float countdown;
    private bool hasExploaded = false;
    
    public GameObject explosionEffect;

    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (isThrow)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && hasExploaded == false)
            {
                Explode();
                hasExploaded = true;
            }
        }

    }

    private void Explode()
    {
        Instantiate(explosionEffect, transform.position,Quaternion.identity);

        Destroy(gameObject);
    }
}

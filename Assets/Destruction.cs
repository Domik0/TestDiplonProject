using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour
{
    public float delay = 3f;

    private float countdown;

    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            Explode();
        }

    }

    private void Explode()
    {
        Debug.Log("ÊÀÁÓÌ");
    }
}

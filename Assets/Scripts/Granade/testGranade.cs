using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGranade : MonoBehaviour
{
    public GameObject Granade;
    public Transform Trans;
    private GameObject Inst;
    public float Power = 500f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (Input.GetMouseButtonDown(0))
        {
            Inst = Instantiate(Granade, Trans.position, transform.rotation);
            Inst.GetComponent<Rigidbody>().AddForce(transform.forward * Power * 2);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Inst = Instantiate(Granade, Trans.position, transform.rotation);
            Inst.GetComponent<Rigidbody>().AddForce(transform.forward * Power);
        }
    }
}
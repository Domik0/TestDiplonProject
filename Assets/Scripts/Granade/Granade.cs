using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    //public float delay = 3f; // �������� ����� �������
    //public float force = 700f; // ���� ������
    //public float radius = 5f; // ������ ������

    //public AudioClip Launch; // ���� �������
    //public AudioClip Explosion; // ���� ������
    //private AudioSource AudioSouce; // ����� �����

    public GameObject explosion; // ������
    private GameObject ExplosiveObject; // ����� ������� ���������� ������ ��� ������

    public string Tag;


    //float countdown; // ������� ������
    bool hasExploded = false;


    void Start()
    {
        //gameObject.AddComponent<AudioSource>(); // ��������� ����� �����
        //AudioSouce = gameObject.GetComponent<AudioSource>(); // ����������� ����� ����� �� ���� � ��� ���
        //countdown = delay; // ������� ������
        //AudioSouce.PlayOneShot(Launch); //���� �������
    }


    void Update()
    {
        //countdown -= Time.deltaTime; // ������� ������

        //if (countdown <= 0f && !hasExploded) //���� ������ ��������
        //{
        //    Explode();
        //    hasExploded = true;
        //}
    }

    void OnCollisionEnter(Collision col)
    {
        Explode();
        hasExploded = true;
    }

    void Explode() // ��� ������
    {

        //show effect
        ExplosiveObject = Instantiate(explosion, transform.position, transform.rotation);
        //ExplosiveObject.AddComponent<AudioSource>(); // ��������� ����� �����
        //ExplosiveObject.GetComponent<AudioSource>().PlayOneShot(Explosion); // ���� ������
        Debug.Log("Boom");

        //Collider[] colliders = Physics.OverlapSphere(transform.position, radius);


        //foreach (Collider nearbyObject in colliders) // ������� �������� � ������ ������
        //{
        //    //��� ����� ���� ���� ������� 
        //    Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
        //    if (rb != null)
        //    {
        //        rb.AddExplosionForce(force, transform.position, radius);
        //    }
        //}

        //destroy Granade
        Destroy(gameObject);
    }
}
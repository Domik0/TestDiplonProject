using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public string name;
    public float delay = 0.3f; // �������� ����� �������
    public float radius = 20f; // ������ ������

    //public AudioClip Launch; // ���� �������
    //public AudioClip Explosion; // ���� ������
    //private AudioSource AudioSouce; // ����� �����

    public GameObject explosion; // ������
    private GameObject ExplosiveObject; // ����� ������� ���������� ������ ��� ������

    public string Tag;


    float countdown; // ������� ������
    bool hasCollision = false;


    void Start()
    {
        //gameObject.AddComponent<AudioSource>(); // ��������� ����� �����
        //AudioSouce = gameObject.GetComponent<AudioSource>(); // ����������� ����� ����� �� ���� � ��� ���
        countdown = delay; // ������� ������
        //AudioSouce.PlayOneShot(Launch); //���� �������
    }


    void Update()
    {
        if (hasCollision)
        {
            countdown -= Time.deltaTime; // ������� ������
        }

        if (countdown <= 0f) //���� ������ ��������
        {
            Explode();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        hasCollision = true;
    }

    void Explode() // ��� ������
    {

        //show effect
        ExplosiveObject = Instantiate(explosion, transform.position, Quaternion.identity);
        //ExplosiveObject.AddComponent<AudioSource>(); // ��������� ����� �����
        //ExplosiveObject.GetComponent<AudioSource>().PlayOneShot(Explosion); // ���� ������
        Debug.Log("Boom");

        if(name == "BombStun")
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider nearbyObject in colliders) // ������� �������� � ������ ������
            {
                //��� ����� ���� ���� ������� 
                var th = nearbyObject.GetComponent<ThirdPersonController>();
                if (th != null)
                {
                    th.StunMove();
                }
            }
        }
        
        Destroy(gameObject);
    }
}
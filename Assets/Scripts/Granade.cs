using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;

public class Granade : NetworkBehaviour
{
    public string name;
    public float delay = 0.3f; // �������� ����� �������
    public float radius = 20f; // ������ ������

    //public AudioClip Launch; // ���� �������
    //public AudioClip Explosion; // ���� ������
    //private AudioSource AudioSouce; // ����� �����

    public NetworkObject explosionSmoke;
    public NetworkObject explosionStun;
    public NetworkObject explosionSlowdown;

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
            Explode(transform.position, name);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        hasCollision = true;
    }

    void Explode(Vector3 position, string name) // ��� ������
    {
        SpawnEffectServerRpc(position, name);

        //ExplosiveObject.AddComponent<AudioSource>(); // ��������� ����� �����
        //ExplosiveObject.GetComponent<AudioSource>().PlayOneShot(Explosion); // ���� ������

        if (name == "BombStun")
        {
            Collider[] colliders = Physics.OverlapSphere(position, radius);

            foreach (Collider nearbyObject in colliders) // ������� �������� � ������ ������
            {

                var th = nearbyObject.GetComponent<ThirdPersonController>();
                if (th != null)
                {
                    StunServerRpc(th.OwnerClientId);
                }
            }
        }

        DestroyBonusServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StunServerRpc(ulong clientId)
    {
        NetworkManager.Singleton.ConnectedClients[clientId]
                .PlayerObject.GetComponent<ThirdPersonController>().StunMove();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnEffectServerRpc(Vector3 position, string nameBonus)
    {
        NetworkObject effectInstance = null;
        if (nameBonus == "BombStun")
        {
            effectInstance = Instantiate(explosionStun, position, Quaternion.identity);
        }
        if (nameBonus == "Smoke")
        {
            effectInstance = Instantiate(explosionSmoke, position, Quaternion.identity);
        }
        if (nameBonus == "Slowdown")
        {
            effectInstance = Instantiate(explosionSlowdown, position, Quaternion.identity);
        }

        effectInstance.Spawn();
    }

    [ServerRpc(RequireOwnership = false)]
    void DestroyBonusServerRpc()
    {
        Destroy(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using Unity.Netcode;
using UnityEngine;

public class Granade : NetworkBehaviour
{
    public string name;
    public float delay = 0.3f; // Задержка перед взрывом
    public float radius = 20f; // Радиус взрыва

    //public AudioClip Launch; // звук запуска
    //public AudioClip Explosion; // звук взрыва
    //private AudioSource AudioSouce; // аудио соурс

    public NetworkObject explosionSmoke;
    public NetworkObject explosionStun;
    public NetworkObject explosionSlowdown;

    float countdown; // скрытый таймер
    bool hasCollision = false;


    void Start()
    {
        //gameObject.AddComponent<AudioSource>(); // Добавляем аудио соурс
        //AudioSouce = gameObject.GetComponent<AudioSource>(); // Присваеваем аудио соурс из игры в наш код
        countdown = delay; // скрытый таймер
        //AudioSouce.PlayOneShot(Launch); //звук запуска
    }


    void Update()
    {
        if (hasCollision)
        {
            countdown -= Time.deltaTime; // скрытый таймер
        }

        if (countdown <= 0f) //Если таймер сработал
        {
            Explode(transform.position, name);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        hasCollision = true;
    }

    void Explode(Vector3 position, string name) // При взрыве
    {
        SpawnEffectServerRpc(position, name);

        //ExplosiveObject.AddComponent<AudioSource>(); // Добавляем аудио соурс
        //ExplosiveObject.GetComponent<AudioSource>().PlayOneShot(Explosion); // звук взрыва

        if (name == "BombStun")
        {
            Collider[] colliders = Physics.OverlapSphere(position, radius);

            foreach (Collider nearbyObject in colliders) // Объекты попавшие в радиус взрыва
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
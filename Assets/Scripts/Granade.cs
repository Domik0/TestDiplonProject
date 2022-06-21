using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class Granade : MonoBehaviour
{
    public string name;
    public float delay = 0.3f; // Задержка перед взрывом
    public float radius = 20f; // Радиус взрыва

    //public AudioClip Launch; // звук запуска
    //public AudioClip Explosion; // звук взрыва
    //private AudioSource AudioSouce; // аудио соурс

    public GameObject explosion; // Эффект
    private GameObject ExplosiveObject; // обект который показывает эффект при взрыве

    public string Tag;


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
            Explode();
        }
    }

    void OnCollisionEnter(Collision col)
    {
        hasCollision = true;
    }

    void Explode() // При взрыве
    {

        //show effect
        ExplosiveObject = Instantiate(explosion, transform.position, Quaternion.identity);
        //ExplosiveObject.AddComponent<AudioSource>(); // Добавляем аудио соурс
        //ExplosiveObject.GetComponent<AudioSource>().PlayOneShot(Explosion); // звук взрыва
        Debug.Log("Boom");

        if(name == "BombStun")
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider nearbyObject in colliders) // Объекты попавшие в радиус взрыва
            {
                //Тут могла быть ваша функция 
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
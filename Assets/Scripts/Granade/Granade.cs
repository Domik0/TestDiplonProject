using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour
{
    //public float delay = 3f; // Задержка перед взрывом
    //public float force = 700f; // Сила взрыва
    //public float radius = 5f; // Радиус взрыва

    //public AudioClip Launch; // звук запуска
    //public AudioClip Explosion; // звук взрыва
    //private AudioSource AudioSouce; // аудио соурс

    public GameObject explosion; // Эффект
    private GameObject ExplosiveObject; // обект который показывает эффект при взрыве

    public string Tag;


    //float countdown; // скрытый таймер
    bool hasExploded = false;


    void Start()
    {
        //gameObject.AddComponent<AudioSource>(); // Добавляем аудио соурс
        //AudioSouce = gameObject.GetComponent<AudioSource>(); // Присваеваем аудио соурс из игры в наш код
        //countdown = delay; // скрытый таймер
        //AudioSouce.PlayOneShot(Launch); //звук запуска
    }


    void Update()
    {
        //countdown -= Time.deltaTime; // скрытый таймер

        //if (countdown <= 0f && !hasExploded) //Если таймер сработал
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

    void Explode() // При взрыве
    {

        //show effect
        ExplosiveObject = Instantiate(explosion, transform.position, transform.rotation);
        //ExplosiveObject.AddComponent<AudioSource>(); // Добавляем аудио соурс
        //ExplosiveObject.GetComponent<AudioSource>().PlayOneShot(Explosion); // звук взрыва
        Debug.Log("Boom");

        //Collider[] colliders = Physics.OverlapSphere(transform.position, radius);


        //foreach (Collider nearbyObject in colliders) // Объекты попавшие в радиус взрыва
        //{
        //    //Тут могла быть ваша функция 
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    private float radius = 5f;
    private float explosionDelay = 5f;
    private float damage = 100f;
    private float force = 4000f;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioClip explosionClip;
    public BotRun thrownBot; //who is throwded this object

    private void Start()
    {
        Invoke("explosion", explosionDelay);
    }

    private void explosion()
    {
        GameObject explosionInstantiated = Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] Botscolliders = Physics.OverlapSphere(transform.position, radius, 1<<8);

        foreach(Collider collider in Botscolliders)
        {
            if (collider.gameObject.layer == 8)
            {
                BotRun colliderScript = collider.GetComponent<BotRun>();
                if (colliderScript.TakeDamage(damage))
                    thrownBot.kill();
                
            }
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, 1 << 0);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(force, transform.position, radius);
        }

            Destroy(explosionInstantiated, 5f);
        Destroy(gameObject);
    }
}

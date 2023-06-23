using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fragGrenade : grenade
{
    private float damage = 100f;
    private float force = 4000f;
    private float radius = 5f;

    protected override void explosion()
    {
        audioPlay();
        GameObject explosionInstantiated = Instantiate(explosionEffect, transform.position, transform.rotation);

        Collider[] Botscolliders = Physics.OverlapSphere(transform.position, radius, 1 << 8);

        foreach (Collider collider in Botscolliders)
        {
            if (collider.gameObject.layer == 8 && collider.gameObject.CompareTag(enemiesTag))
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


        foreach (Transform children in transform)
        {
            children.gameObject.SetActive(false);
        }
        Destroy(explosionInstantiated, 5f);
        Destroy(gameObject, 3f); //delay in destroy needs for audioPlay to end
    }
}

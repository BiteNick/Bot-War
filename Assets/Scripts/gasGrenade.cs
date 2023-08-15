using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gasGrenade : grenade
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float radius = 10f;
    [SerializeField] private float lifeSpan = 12f; //Время жизни
    [SerializeField] private float timeStep = 1f;
    protected override void explosion()
    {
        audioPlay();
        StartCoroutine("smog");
        GameObject explosionInstantiated = Instantiate(explosionEffect, transform.position + new Vector3(0, 2.5f, 0), transform.rotation);
        Destroy(explosionInstantiated, lifeSpan + 2);
        Destroy(gameObject, lifeSpan);
    }

    private IEnumerator smog()
    {
        while (lifeSpan > 0)
        {
            Collider[] Botscolliders = Physics.OverlapSphere(transform.position, radius, 1 << 8);

            foreach (Collider collider in Botscolliders)
            {
                if (collider.gameObject.layer == 8 && collider.gameObject.CompareTag(enemiesTag))
                {
                    BotRun colliderScript = collider.GetComponent<BotRun>();
                    colliderScript.TakeDamage(damage);
                    colliderScript.createGasEffect(timeStep);
                }
            }
            lifeSpan -= timeStep;
            yield return new WaitForSeconds(timeStep);
        }
    }

}

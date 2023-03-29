using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK : Gun
{

    override protected void FireWithDelay()
    {
        if (delay <= 0)
        {
            RaycastHit hit;
            Ray ray = new Ray(SpawnBulletPos.position, SpawnBulletPos.transform.forward);

            Vector3 targetPoint;
            if (spread - character.spreadDebuff > 0)
            {
                ray.direction = Quaternion.Euler(0, Random.Range(-spread + character.spreadDebuff, spread - character.spreadDebuff), 0) * transform.right;
            }
            else
            {
                ray.direction = Quaternion.Euler(0, 0, 0) * transform.right;

            }

            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point;
                if (hit.transform.tag.Substring(0, 3) == "bot")
                {
                    BotRun botRun = hit.transform.gameObject.GetComponent<BotRun>();
                    if (botRun.TakeDamage(damage)) //Проверка на убийство врага
                    {
                        character.kill();
                    }

                }
            }

            else
            {
                return;
            }

            if (hit.transform.tag.Substring(0, 3) == "bot")
            {
                particleEffect = robotParticleEffect;
            }
            else if(hit.transform.CompareTag("parapet"))
            {
                particleEffect = parapetParticleEffect;
            }
            else if (hit.transform.CompareTag("terrain"))
            {
                particleEffect = terrainParticleEffect;
            }
            else
            {
                particleEffect = null;
            }

            if (particleEffect != null)
            {
                GameObject hitParticle = Instantiate(particleEffect, targetPoint, Quaternion.Euler(ray.direction));
                Destroy(hitParticle, 2f);
                if (hit.transform.tag == "parapet" || hit.transform.tag == "terrain")
                {
                    if (Random.Range(0, 100) > 70)
                    {
                        AudioSource hitParticleAudio = hitParticle.GetComponent<AudioSource>();
                        hitParticleAudio.PlayOneShot(rikoshetClips[Random.Range(0, rikoshetClips.Length)]);
                    }
                }
            }


            GameObject muzzleFlashEffect = Instantiate(muzzleFlash, SpawnBulletPos.position, Quaternion.identity, SpawnBulletPos);
            muzzleFlashEffect.transform.localRotation = Quaternion.Euler(0, 0, 0);
            Destroy(muzzleFlashEffect, 3f);
            GameObject sparks = Instantiate(sparksEffect, SpawnBulletPos.position, Quaternion.identity, SpawnBulletPos);
            sparks.transform.localRotation = Quaternion.Euler(0, 90, 0);
            sparks.transform.parent = null;
            Destroy(sparks, 1f);

            
                

            audioSource.PlayOneShot(shootClip);
            
            anim.SetTrigger("Attack");
            delay = FireRate;
        }
    }

    public void PickUp(Transform player)
    {
        transform.parent = player.transform;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        transform.Rotate(0f, offsetY, 0f);
        transform.localPosition = new Vector3(0.4f, -0.25f, 0.4f);
    }

}

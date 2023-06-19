using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shotGun : Gun
{
    [SerializeField] private int shotsLength = 6;
    protected override Vector3 GetForwardVector()
    {
        return transform.forward * -1;
    }

    new protected void FireWithDelay()
    {
        if (delay <= 0 && magazineAmmos > 0)
        {
            for (int i = 0; i < shotsLength; i++)
            {

                RaycastHit hit;
                Ray ray = new Ray(SpawnBulletPos.position, SpawnBulletPos.transform.forward);

                Vector3 targetPoint;
                if (spread - character.spreadDebuff > 0)
                {
                    ray.direction = Quaternion.Euler(0, Random.Range(-spread + character.spreadDebuff, spread - character.spreadDebuff), 0) * GetForwardVector();
                }
                else
                {
                    ray.direction = Quaternion.Euler(0, 0, 0) * GetForwardVector();
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
                            character.target = null;
                        }

                    }

                    if (hit.transform.tag.Substring(0, 3) == "bot")
                    {
                        particleEffect = robotParticleEffect;
                    }
                    else if (hit.transform.CompareTag("parapet"))
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
                                if (rikoshetClips.Length > 0)//TEMP
                                    hitParticleAudio.PlayOneShot(rikoshetClips[Random.Range(0, rikoshetClips.Length)]);
                            }
                        }
                    }
                }
                
            }


            GameObject muzzleFlashEffect = Instantiate(muzzleFlash, SpawnBulletPos.position, Quaternion.identity, SpawnBulletPos);
            muzzleFlashEffect.transform.localRotation = Quaternion.Euler(0, 0, 0);
            Destroy(muzzleFlashEffect, 3f);
            GameObject sparks = Instantiate(sparksEffect, SpawnBulletPos.position, Quaternion.identity, SpawnBulletPos);
            sparks.transform.localRotation = Quaternion.Euler(0, 90, 0);
            sparks.transform.parent = null;
            ParticleSystem.ShapeModule ps = sparks.GetComponent<ParticleSystem>().shape;
            ps.angle = spread - character.spreadDebuff;

            audioSource.PlayOneShot(shootClip);
            delay = FireRate;
            magazineAmmos--;


            if (magazineAmmos <= 0 && !isReloading)
            {
                isReloading = true;
                character.ReloadStart(); //Начало перезарядки
            }
        }

    }

}

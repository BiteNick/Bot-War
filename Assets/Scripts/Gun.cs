using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    protected GameObject particleEffect = null;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] shootClips;
    [SerializeField] protected AudioClip shootClip;
    [SerializeField] protected AudioClip[] rikoshetClips;
    [SerializeField] protected GameObject terrainParticleEffect;
    [SerializeField] protected GameObject parapetParticleEffect;
    [SerializeField] protected GameObject robotParticleEffect;
    [SerializeField] protected GameObject muzzleFlash;
    [SerializeField] protected GameObject sparksEffect;
    [SerializeField] public GameObject MagazineObject;
    [SerializeField] private GameObject currentMagazineObject;
    [SerializeField] private GameObject fakeMagazineObject; //������������ ����� ������� ������������� � �����������, ����� ������� ��������������
    [SerializeField] public Transform SpawnBulletPos;
    [SerializeField] public float fireDistance; //��������� ��������
    [SerializeField] protected float spread; //��������� �������
    [SerializeField] protected float damage; //����
    [SerializeField] public float FireRate; //�������� ����� ����������
    [SerializeField] protected int magazineCapacity = 1; //����������� ��������
    [SerializeField] protected int magazineAmmos; //������� ���������� ��������
    protected float delay; //�������� �� ����� ���������� ��������
    protected float offsetY = -90; //����������� �������� ������
    protected Animator anim;
    protected BotRun character;
    [SerializeField] protected float fireDelay = 0.1f; //����� ������� �������� ��������, ����� ��������
    protected bool isReloading = false;
    public bool firstContact; //if it is gun like shotGun
    void Start()
    {
        if (MagazineObject != null)
        {
            currentMagazineObject = transform.Find("Magazine").gameObject;
            fakeMagazineObject = transform.Find("fakeMagazine").gameObject;
        }
        magazineAmmos = magazineCapacity;
        if (shootClips.Length > 0)
        {
            shootClip = shootClips[Random.Range(0, shootClips.Length)];
        }
        if (gameObject.transform.parent == null)
        {
            this.enabled = false;
        }
        anim = GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }


    public void Fire()
    {
        Invoke("FireWithDelay", fireDelay);//����� �������� � ��������� (������� �� �������� ��������, ����� ����������� ��������)
    }

    protected void FireWithDelay()
    {
        if (delay <= 0 && magazineAmmos > 0)
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
                    if (botRun.TakeDamage(damage)) //�������� �� �������� �����
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

            
            GameObject muzzleFlashEffect = Instantiate(muzzleFlash, SpawnBulletPos.position, Quaternion.identity, SpawnBulletPos);
            muzzleFlashEffect.transform.localRotation = Quaternion.Euler(0, 0, 0);
            Destroy(muzzleFlashEffect, 3f);
            GameObject sparks = Instantiate(sparksEffect, SpawnBulletPos.position, Quaternion.identity, SpawnBulletPos);
            sparks.transform.localRotation = Quaternion.Euler(0, 90, 0);
            sparks.transform.parent = null;
            ParticleSystem.ShapeModule ps = sparks.GetComponent<ParticleSystem>().shape;
            ps.angle = spread - character.spreadDebuff;

            
            Destroy(sparks, 1f);




            audioSource.PlayOneShot(shootClip);

            animSetTrigger("Attack");
            delay = FireRate;
            magazineAmmos--;
        }
        if(magazineAmmos <= 0 && !isReloading)
        {
            isReloading = true;
            character.ReloadStart(); //������ �����������
        }
    }


    public void MagazineDetach() //������������ ��������
    {
        if (MagazineObject != null)
        {
            currentMagazineObject.transform.parent = null;
            currentMagazineObject.GetComponent<Rigidbody>().isKinematic = false;
            currentMagazineObject.GetComponent<BoxCollider>().enabled = true;
            Destroy(currentMagazineObject, 10f);
        }
    }

    public void MagazinePickUp(GameObject magazine)
    {
        currentMagazineObject = magazine;
    }

    public void MagazineAttach()
    {
        currentMagazineObject.transform.parent = this.gameObject.transform;
        currentMagazineObject.transform.rotation = fakeMagazineObject.transform.rotation;
        currentMagazineObject.transform.position = fakeMagazineObject.transform.position;
        currentMagazineObject.transform.localScale = fakeMagazineObject.transform.localScale;
        currentMagazineObject.transform.localPosition = fakeMagazineObject.transform.localPosition;
    }

    public void refillMagazine() //����� �����������
    {
        magazineAmmos = magazineCapacity;
        isReloading = false;
    }


    protected abstract Vector3 GetForwardVector();


    public void setCharacter(BotRun character)
    {
        this.character = character;
    }

    private void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
        }
    }


    public void animSetTrigger(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }


}

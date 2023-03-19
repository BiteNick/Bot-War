using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
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
    [SerializeField] public Transform SpawnBulletPos;
    [SerializeField] protected float fireDistance; //��������� ��������
    [SerializeField] protected float spread; //��������� �������
    [SerializeField] protected float damage; //����
    [SerializeField] public float FireRate; //�������� ����� ����������
    protected float delay; //�������� �� ����� ���������� ��������
    protected float offsetY = -90; //����������� �������� ������
    protected Animator anim;
    void Start()
    {
        shootClip = shootClips[Random.Range(0, shootClips.Length)];
        if (gameObject.transform.parent == null)
        {
            this.enabled = false;
        }
        anim = GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public virtual void Fire(BotRun character) { }

    private void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class grenade : MonoBehaviour
{
    [SerializeField] private float explosionDelay = 5f;
    [SerializeField] protected GameObject explosionEffect;
    [SerializeField] protected AudioClip explosionClip;
    protected BotRun thrownBot; //who is throwded this object
    protected string enemiesTag;

    private void Start()
    {
        Invoke("explosion", explosionDelay);
    }

    public void GrenadeInit(BotRun thrownBot, string enemiesTag)
    {
        this.thrownBot = thrownBot;
        this.enemiesTag = enemiesTag;
    }

    protected abstract void explosion();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsMove : MonoBehaviour
{
    private Vector3 positionOffset;
    private BotRun character;
    private bool isSprite;
    private bool isAnim;
    private Transform Camera;


    public void myConstructor(Vector3 positionOffset, BotRun character, bool isSprite, bool isAnim)
    {
        this.positionOffset = positionOffset;
        this.character = character;
        this.isSprite = isSprite;
        this.isAnim = isAnim;
        Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (isSprite && isAnim)
            Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        else
            Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.position = character.transform.position + positionOffset;
        if (isSprite)
            transform.LookAt(Camera);
    }
}

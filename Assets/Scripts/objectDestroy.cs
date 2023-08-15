using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectDestroy : MonoBehaviour
{
    [SerializeField] private float destroyTime;
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            Invoke("destroyObject", anim.GetCurrentAnimatorStateInfo(0).length);
        else
            Invoke("destroyObject", destroyTime);
    }

    private void destroyObject()
    {
        Destroy(gameObject);
    }

}

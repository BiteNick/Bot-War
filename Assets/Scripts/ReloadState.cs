using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/ReloadState")]
public class ReloadState : State
{
    override public void preInit()
    {
        StateName = "ReloadState";
    }
    override public void Init()
    {
        character.animSetBool("isReloading");
    }
    override public void Run()
    {

    }
}

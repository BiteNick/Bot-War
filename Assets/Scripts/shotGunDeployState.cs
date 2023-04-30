using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/shotGunDeployState")]
public class shotGunDeployState : State
{
    public override void preInit()
    {
        StateName = "shotGunDeployState";
    }

    public override void Init()
    {
        character.animSetTrigger("firstContact");
        character.animClearBools();
        character.Gun.firstContact = false;
    }

    public override void Run()
    {

    }
}

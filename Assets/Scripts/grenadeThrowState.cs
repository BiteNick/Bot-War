using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/grenadeThrowState")]
public class grenadeThrowState : State
{
    private float timeToNextState = 3f;
    public override void preInit()
    {
        StateName = "grenadeThrowState";
    }
    
    public override void Init()
    {
        character.animSetBool("isGrenading");
        character.EnableRigBuilder(true);

    }

    public override void Run()
    {
        timeToNextState -= Time.deltaTime;
        if (timeToNextState < 0)
            character.CheckStayState();
    }
}

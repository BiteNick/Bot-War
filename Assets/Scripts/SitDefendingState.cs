using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/SitDefendingState")]
public class SitDefendingState : State
{
    private float delayShootSample;
    private float delayShoot;
    private float timerToMoveStateMax = 0.5f; //Таймер для переключения в состояние бега если никого нету
    private float timerToMoveState; //осталось времени для переключения состояния

    private Vector3 sitColliderSize = new Vector3(1f, 2f, 1f);
    private Vector3 sitColliderCenter = new Vector3(0f, 1f, 0f);

    public override void preInit()
    {
        StateName = "SitDefendingState";
    }

    public override void Init()
    {
        character.SetColliderSize(sitColliderSize, sitColliderCenter);
        timerToMoveState = timerToMoveStateMax;
        character.animSetBool("isSitting");
        delayShootSample = character.delayShot;
        character.spreadDebuff = 2;
    }

    public override void Run()
    {
        if (character.target != null)
        {

            if (delayShoot < 0f)
            {
                character.Gun.Fire();
                delayShoot = delayShootSample;
            }
            else
            {
                delayShoot -= Time.deltaTime;
            }
            timerToMoveState = timerToMoveStateMax;
        }
        else if (character.currentPositionGameObject == null || !character.MovedToPosition)
        {
            timerToMoveState -= Time.deltaTime;
            if (timerToMoveState <= 0f)
            {
                character.SetState(character.MoveState);
            }

        }
    }
}

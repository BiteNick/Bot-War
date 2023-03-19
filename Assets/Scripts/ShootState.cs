using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[CreateAssetMenu(menuName = "States/ShootState")]
public class ShootState : State
{
    private float delayShootSample;
    private float delayShoot;
    private float timerToMoveStateMax = 0.5f; //������ ��� ������������ � ��������� ���� ���� ������ ����
    private float timerToMoveState; //�������� ������� ��� ������������ ���������

    override public void preInit()
    {
        StateName = "ShootState";
    }

    override public void Init()
    {
        timerToMoveState = timerToMoveStateMax;
        delayShootSample = character.delayShot;
        character.animSetBool("isShooting");

    }

    override public void Run()
    {
        if (character.target != null)
        {

            if (delayShoot < 0f)
            {
                character.Gun.Fire(character);
                delayShoot = delayShootSample;
            }
            else
            {
                delayShoot -= Time.deltaTime;
            }
            timerToMoveState = timerToMoveStateMax;
        }
        else if (!character.StayAtPosition && character.currentPositionGameObject == null || !character.MovedToPosition && !character.StayAtPosition)
        {
            timerToMoveState -= Time.deltaTime;
            if (timerToMoveState <= 0f)
            {
                character.SetState(character.MoveState);
            }

        }
    }


}

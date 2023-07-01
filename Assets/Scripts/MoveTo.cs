using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/MoveState")]
public class MoveTo : State
{
    private Transform movePoint;
    public float mapBeginningZ;
    public float mapEndingZ;
    public float nonShelterRunDistance = 10f; //расстояние от которого персонаж может бежать до ближайшего укрытия
    private float leftDistance; //calculates every frame

    public override void preInit()
    {
        StateName = "MoveState";
    }

    public override void Init()
    {
        if (character.EnemiesTag == "bot_enemy")
            mapEndingZ = gameManagerStatic.currentMapWidthZ;

        else
            mapEndingZ = 0;


        if (character.skipPosition)
        {
            character.skipPosition = false;
        }
        else if(character.currentPositionGameObject != null && character.MovedToPosition)
        {
            gameManagerStatic.SetPosition(character.currentPositionGameObject, false, character.gameObject);
        }

        character.currentPositionGameObject = null;
        character.MovedToPosition = false;
        character.EnableRigBuilder(false);
        character.animSetBool("isRunning");
        character.animSetLegsState(0);

        foreach (GameObject stand in gameManagerStatic.Positions.Keys)
        {
            if (gameManagerStatic.positionCheck(character.EnemiesTag, stand) && positionCountX(stand.transform, character.transform) && positionCountZ(stand.transform, character.transform))
            {
                movePoint = stand.transform;
                character.currentPositionGameObject = stand;
                break;
            }
        }

        if (character.currentPositionGameObject == null)
        {
            movePoint = new GameObject().transform;
            movePoint.transform.parent = character.transform;
            movePoint.transform.name = "MovePoint";
            movePoint.position = new Vector3(character.transform.position.x, character.transform.position.y, mapEndingZ);
        }
        character.setMovePosition(movePoint.position);

    }

    public override void Run()
    {
        leftDistance = (character.transform.position - movePoint.position).magnitude;
        if (leftDistance < 1f)
        {
            

            if (gameManagerStatic.Positions[character.currentPositionGameObject] == true)
            {
                foreach (GameObject stand in gameManagerStatic.positionsGroups[character.currentPositionGameObject.transform.parent.gameObject])
                {
                    if (gameManagerStatic.Positions[stand] == false)
                    {
                        character.currentPositionGameObject = stand;
                        character.setMovePosition(stand.transform.position);
                        movePoint = stand.transform;
                        character.skipPosition = true;
                        return;
                    }
                }

                character.currentPositionGameObject = null;
                character.skipPosition = true;
                character.SetState(character.MoveState);
            }

            else
            {
                gameManagerStatic.SetPosition(character.currentPositionGameObject, true, character.gameObject);
                if (character.CompareTag("bot_ally"))
                {
                    gameManagerStatic.turnButtons(character.currentPositionGameObject);
                }
                character.MovedToPosition = true;
                character.SetState(character.DefendState);
            }

        }

        else if (leftDistance < 5f)
        {
            if (gameManagerStatic.Positions[character.currentPositionGameObject] == true)
            {
                foreach (GameObject stand in gameManagerStatic.positionsGroups[character.currentPositionGameObject.transform.parent.gameObject])
                {
                    if (gameManagerStatic.Positions[stand] == false)
                    {
                        character.currentPositionGameObject = stand;
                        character.setMovePosition(stand.transform.position);
                        movePoint = stand.transform;
                        return;
                    }
                }
            }
        }
    }

    private bool positionCountX(Transform markerPosition, Transform characterPosition) //делает ограничение на поиск по сторонам
    {
        return Mathf.Abs(markerPosition.position.x - characterPosition.position.x) <= 10f;
    }

    private bool positionCountZ(Transform standTransform, Transform characterTransform) //чтобы бежали только вперёд относительно своей стороны
    {
        if (character.CompareTag("bot_ally"))
            return standTransform.position.z - characterTransform.position.z > nonShelterRunDistance;
        else if (character.CompareTag("bot_enemy"))
            return characterTransform.position.z - standTransform.position.z > nonShelterRunDistance;
        else
        {
            Debug.Log("here is it a problem");
            return false;
        }
    }

}

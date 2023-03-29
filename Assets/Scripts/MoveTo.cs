using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "States/MoveState")]
public class MoveTo : State
{
    private Transform movePoint;
    public float mapBeginningZ;
    public float mapEndingZ;
    public float nonShelterRunDistance = 10f; //расстояние от которого персонаж может бежать до близжайшего укрытия

    public override void preInit()
    {
        StateName = "MoveState";
    }

    public override void Init()
    {
        character.MovedToPosition = false;
        character.ClearTarget();
        character.animSetBool("isRunning");

        foreach (GameObject stand in gameManagerStatic.Positions.Keys)
        {
            if (gameManagerStatic.Positions[stand] == false && positionCountX(stand.transform, character.transform) && positionCountZ(stand.transform, character.transform))
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
        if ((character.transform.position - movePoint.position).magnitude < 1f)
        {
            character.MovedToPosition = true;
            character.SetState(character.DefendState);

            if (character.CompareTag("bot_ally"))
            {
                gameManagerStatic.turnButtons(character.currentPositionGameObject);
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

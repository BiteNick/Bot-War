using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class gameManagerStatic
{
    //positions
    public static float currentMapWidthX;
    public static float currentMapWidthZ;

    //settings
    public static int GraphicsQuality;
    public static float Volume = 0.7f;
    public static float CameraMaxKeysMovingSpeed = 8f;
    public static float CameraKeysMovingSpeed = 4f;
    public static float CameraMaxMoveSpeed = 8f;
    public static float CameraMoveSpeed = 4f;
    public static float CameraMaxScrollSpeed = 4f;
    public static float CameraScrollSpeed = 2f;


    //positions
    public static Dictionary<GameObject, bool> Positions; //engaged positions (true - if engaged, false - if empty)
    public static List<GameObject> positionsGroupAlly;
    public static List<GameObject> positionsGroupEnemy;
    public static List<GameObject> positionsGroupCommon;


    //List stats of characters
    public static List<BotRun> botRunStatsAlly = new List<BotRun>(); //0 - ak, 1 - machinegun, 2 - sniper, 3 - shotgun


    public static Dictionary<GameObject, List<GameObject>> positionsGroups;
    public static void StartManager()
    {
        Positions = new Dictionary<GameObject, bool>();


        positionsGroupAlly = new List<GameObject>(); //groups of positions ally
        positionsGroupAlly.AddRange(GameObject.FindGameObjectsWithTag("positionsGroupAlly"));

        positionsGroupEnemy = new List<GameObject>(); //groups of positions enemy
        positionsGroupEnemy.AddRange(GameObject.FindGameObjectsWithTag("positionsGroupEnemy"));

        positionsGroupCommon = new List<GameObject>(); //groups of common positions
        positionsGroupCommon.AddRange(GameObject.FindGameObjectsWithTag("positionsGroup"));

        List<GameObject> allPositionsGroup = new List<GameObject>();
        allPositionsGroup.AddRange(positionsGroupAlly);
        allPositionsGroup.AddRange(positionsGroupEnemy);
        allPositionsGroup.AddRange(positionsGroupCommon);


        positionsGroups = new Dictionary<GameObject, List<GameObject>>();


        foreach (GameObject positionsGroup in allPositionsGroup)
        {
            positionsGroups[positionsGroup] = new List<GameObject>();
            foreach (Transform position in positionsGroup.transform)
            {
                if (position.CompareTag("position"))
                {
                    positionsGroups[positionsGroup].Add(position.gameObject);
                }
            }
        }
        
        foreach (GameObject child in GameObject.FindGameObjectsWithTag("position"))
        {
            Positions.Add(child, false);
        }
            turnButtons();
    }


    public static void SetPosition(GameObject stand, bool boolValue, GameObject character)
    {
        Positions[stand] = boolValue;
    }

    public static void turnButtons() //Метод, просматривающий весь список и включающий/выключающий группу кнопок позиций если они заняты/пусты
    {
        foreach (KeyValuePair<GameObject, List<GameObject>> dictItem in positionsGroups)
        {
            bool isTrue = false;
            foreach (GameObject item in dictItem.Value)
            {
                if (Positions[item] == true)
                {
                    isTrue = true;
                    break;
                }
            }
            if (isTrue)
            {
                dictItem.Key.gameObject.SetActive(true);
            }
            else
            {
                dictItem.Key.gameObject.SetActive(false);
            }
        }
    }

    public static void turnButtons(GameObject position) //Метод, просматривающий весь список и включающий/выключающий группу кнопок позиций если они заняты/пусты
    {
        foreach (KeyValuePair<GameObject, List<GameObject>> dictItem in positionsGroups)
        {
            bool isTrue = false;
            bool matchPositions = false;
            foreach (GameObject item in dictItem.Value)
            {
                
                if (Positions[item] == true)
                {
                    isTrue = true;
                }
                if (position == item)
                {
                    matchPositions = true;
                }
            }
            if (matchPositions)
            {
                if (isTrue)
                {
                    dictItem.Key.gameObject.SetActive(true);
                }
                else
                {
                    dictItem.Key.gameObject.SetActive(false);
                }
                break;
            }
            
        }
    }


    public static void turnOffButtons(GameObject position)
    {
        foreach (KeyValuePair<GameObject, List<GameObject>> dictItem in positionsGroups)
        {
            bool isTrue = false;
            bool matchPositions = false;
            foreach (GameObject item in dictItem.Value)
            {

                if (Positions[item] == true)
                {
                    isTrue = true;
                }
                if (position == item)
                {
                    matchPositions = true;
                }
            }
            if (matchPositions)
            {
                if (!isTrue)
                {
                    dictItem.Key.gameObject.SetActive(false);
                }
                break;
            }

        }
    }

    public static void Temp()
    {
        foreach (var temp in Positions.Keys)
        {

            Debug.Log($"name: {temp.name}, bool: {Positions[temp]}");
        }
    }

    
    public static bool positionCheck(string enemiesTag, GameObject stand) //checking separation of positions for every team
    {
        if (enemiesTag == "bot_enemy")
        {
            if (groupPositionCheck(positionsGroupAlly, stand) && Positions[stand] == false || groupPositionCheck(positionsGroupCommon, stand) && Positions[stand] == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        else if (enemiesTag == "bot_ally")
        {
            if (groupPositionCheck(positionsGroupEnemy, stand) && Positions[stand] == false || groupPositionCheck(positionsGroupCommon, stand) && Positions[stand] == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        else
        {
            return false;
        }

    }


    private static bool groupPositionCheck(List<GameObject> groupsList, GameObject stand)
    {
        foreach (GameObject group in groupsList)
        {
            foreach (Transform item in group.transform)
            {
                if (item.gameObject == stand)
                {
                    return true;
                }
            }
        }
        return false;
    }

}

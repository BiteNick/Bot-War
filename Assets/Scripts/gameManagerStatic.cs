using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class gameManagerStatic
{
    public static float Volume = 0.7f;
    public static float CameraMaxKeysMovingSpeed = 8f;
    public static float CameraKeysMovingSpeed = 4f;
    public static float CameraMaxMoveSpeed = 8f;
    public static float CameraMoveSpeed = 4f;
    public static Dictionary<GameObject, bool> Positions; //engaged positions (true - if engaged, false - if empty)
    private static Dictionary<GameObject, List<GameObject>> positionsGroups;
    public static void StartManager()
    {
        Positions = new Dictionary<GameObject, bool>();
        positionsGroups = new Dictionary<GameObject, List<GameObject>>();

        foreach (GameObject positionsGroup in GameObject.FindGameObjectsWithTag("positionsGroup"))
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

    


}

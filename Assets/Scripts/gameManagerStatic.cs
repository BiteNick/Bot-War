using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class gameManagerStatic
{
    public static Dictionary<GameObject, bool> Positions = new Dictionary<GameObject, bool>();
    public static void StartManager()
    {
        GameObject positionsParent = GameObject.Find("Positions");
        foreach (Transform child in positionsParent.transform.GetComponentsInChildren<Transform>())
        {
            if (child.name != "Positions")
            {
                Positions.Add(child.gameObject, false);
            }
        }
    }

    public static void SetPosition(GameObject stand, bool boolValue, GameObject character)
    {
        Positions[stand] = boolValue;
    }

    public static void Temp()
    {
        foreach (var temp in Positions.Keys)
        {

            Debug.Log($"name: {temp.name}, bool: {Positions[temp]}");
        }
    }
}

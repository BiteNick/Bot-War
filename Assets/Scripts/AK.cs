using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK : Gun
{
    protected override Vector3 GetForwardVector()
    {
        return transform.right;
    }
    public void PickUp(Transform player)
    {
        transform.parent = player.transform;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        transform.Rotate(0f, offsetY, 0f);
        transform.localPosition = new Vector3(0.4f, -0.25f, 0.4f);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class machineGun : Gun
{
    protected override Vector3 GetForwardVector()
    {
        return transform.forward;
    }
}

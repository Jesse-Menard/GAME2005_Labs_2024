using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlane : PhysicsObject
{
    public bool isHalspace = false;
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Vector3 GetNormal()
    {
        return transform.up;
    }
}

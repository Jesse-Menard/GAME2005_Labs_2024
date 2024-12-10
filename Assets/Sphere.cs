using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : PhysicsObject
{
    [Min(0.001f)]
    public float radius = 1;

    private void OnValidate() // Editor only
    {
       transform.localScale = new Vector3(radius, radius, radius) * 2f;
    }

    private void Update() // Editor only
    {
       transform.localScale = new Vector3(radius, radius, radius) * 2f;
    }
}

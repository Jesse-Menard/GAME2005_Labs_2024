using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public bool enableGravity = true;
    [Range(0f, 2f)]
    public float drag = 0.1f;
    public float mass = 5.0f;
    public float friction = 0.02f;
    public Vector3 velocity = Vector3.zero;

    public Vector3 FGravity;
    public Vector3 FNormal;
    public Vector3 FFriction;
    public Vector3 FNet;

    // Start is called before the first frame update
    void Start()
    {
        PhysicsEngine.Instance.physicsObjects.Add(this);
    }
}
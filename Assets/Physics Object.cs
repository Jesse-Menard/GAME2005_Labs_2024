using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public bool isStatic = false;
    public bool velEqRot = false;
    public float gravityScale = 1.0f;
    //  [Range(0f, 2f)]
    //  public float drag = 0.1f;
    [Range(0f, 1f)]
    public float friction = 0.02f;
    [Range(0f, 1f)]
    public float bounciness = 0.5f;
    [Range(0.0001f, 10000f)]
    public float mass = 1.0f;
    public Vector3 velocity = Vector3.zero;

    public Vector3 FGravity = Vector3.zero;
    public Vector3 FNormal = Vector3.zero;
    public Vector3 FFriction = Vector3.zero;
    public Vector3 FNet;

    [HideInInspector]
    public Vector3 initialVelocity = Vector3.zero;
    [HideInInspector]
    public Vector3 initialPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialVelocity = velocity;

        PhysicsEngine.Instance.physicsObjects.Add(this);
    }
}
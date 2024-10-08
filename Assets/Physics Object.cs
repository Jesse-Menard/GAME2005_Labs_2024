using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float mass = 1;
    [Range(0f, 1f)]
    public float drag = 0.5f;
    public Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        PhysicsEngine.Instance.physicsObjects.Add(this);
    }
}



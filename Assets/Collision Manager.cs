using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    static CollisionManager instance = null;
    public static CollisionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<CollisionManager>();
            }
            return instance;
        }
    }


    public List<PhysicsObject> physicsObjects = PhysicsEngine.Instance.physicsObjects;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (physicsObjects != PhysicsEngine.Instance.physicsObjects)
        {
            physicsObjects = PhysicsEngine.Instance.physicsObjects;
        }

        foreach (PhysicsObject object1 in physicsObjects)
        {
            object1.GetComponent<Renderer>().material.color = Color.white;
        }


    }


}

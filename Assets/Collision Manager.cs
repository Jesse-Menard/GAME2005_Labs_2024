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

        for (int a = 0; a < physicsObjects.Count; a++)
        {
            PhysicsObject object1 = physicsObjects[a];

            for (int b = a + 1; b < physicsObjects.Count; b++)
            {
                PhysicsObject object2 = physicsObjects[b];

                if (object1.GetType() == typeof(Sphere) && object2.GetType() == typeof(Sphere))
                {
                    if (SphereSphereCollision(object1 as Sphere, object2 as Sphere))
                    {
                        // Colliding

                        object1.GetComponent<Renderer>().material.color = new Color(180f / 255f, 0f, 1f);
                        object2.GetComponent<Renderer>().material.color = new Color(180f / 255f, 0f, 1f);
                    }
                }
            }
        }
    }

    bool SphereSphereCollision(Sphere ob1, Sphere ob2)
    {
        float distance = (ob1.transform.position - ob2.transform.position).magnitude;
        return distance < (ob1.radius + ob2.radius);
    }
}

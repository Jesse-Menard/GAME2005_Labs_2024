using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PhysicsEngine : MonoBehaviour
{
    static PhysicsEngine instance = null;
    public static PhysicsEngine Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<PhysicsEngine>();
            }
            return instance;
        }
    }

    public List<PhysicsObject> physicsObjects = new List<PhysicsObject>();
    float time = 0;
    float dt = 0.01667f;
    public Vector3 gravityAcceleration = new Vector3(0.0f, -10.0f, 0.0f);

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (PhysicsObject object1 in physicsObjects)
        {
            Vector3 prevPos = object1.transform.position;
            Vector3 newPos = object1.transform.position + object1.velocity * dt;

            // Update Position on Velocity
            object1.transform.position = newPos;

            // Update Velocity on Accelleration
            Vector3 accelerationThisFrame = gravityAcceleration;

            Vector3 vSquared = object1.velocity.normalized * object1.velocity.sqrMagnitude;

            Vector3 dragacceleration = -object1.drag * vSquared;

            accelerationThisFrame += dragacceleration;

            object1.velocity += accelerationThisFrame * dt;

            // Update Velocity on drag
            //object1.velocity *=  Mathf.Abs(object1.drag - 1); // 0-1

            // Visualize
            Debug.DrawLine(prevPos, newPos, new Color(180.0f/255.0f, 0.0f, 1.0f), 10);
            Debug.DrawLine(object1.transform.position, object1.transform.position + object1.velocity, Color.red);

            time += dt;
        }

        foreach (PhysicsObject object1 in physicsObjects)
        {
            object1.GetComponent<Renderer>().material.color = Color.white;
        }
        
        for(int a = 0; a < physicsObjects.Count; a++)
        {
            PhysicsObject object1 = physicsObjects[a];

            for(int b = a + 1; b < physicsObjects.Count; b++)
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

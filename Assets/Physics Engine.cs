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
        foreach (PhysicsObject Object1 in physicsObjects)
        {
            Vector3 prevPos = Object1.transform.position;
            Vector3 newPos = Object1.transform.position + Object1.velocity * dt;

            // Update Position on Velocity
            Object1.transform.position = newPos;

            // Update Velocity on Accelleration
            Object1.velocity += gravityAcceleration * dt;

            // Update Velocity on drag
            // Object1.velocity += drag * dt ?

            // Visualize
            Debug.DrawLine(prevPos, newPos, new Color(180.0f/255.0f, 0.0f, 1.0f), 10);
            Debug.DrawLine(Object1.transform.position, Object1.transform.position + Object1.velocity, Color.red);

            time += dt;
        }
    }
}

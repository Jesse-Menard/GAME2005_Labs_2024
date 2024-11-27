using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
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
        foreach (PhysicsObject obj in physicsObjects)
        {
            obj.GetComponent<Renderer>().material.color = Color.white;
        }

        KinematicsUpdate();
        CollisionUpdate();
        foreach (PhysicsObject obj in physicsObjects)
        {
            DrawForces(obj);
        }
    }

    private void KinematicsUpdate()
    {
        foreach (PhysicsObject object1 in physicsObjects)
        {
            Vector3 prevPos = object1.transform.position;
            Vector3 newPos = object1.transform.position + object1.velocity * dt;

            // Update Position on Velocity
            object1.transform.position = newPos;

            // Update Velocity on Accelleration
            if (object1.enableGravity)
            {
                object1.FGravity = object1.mass * gravityAcceleration;
                object1.FNet = object1.FFriction + object1.FGravity + object1.FNormal;

                Vector3 accelerationThisFrame = gravityAcceleration;

                Vector3 vSquared = object1.velocity.normalized * object1.velocity.sqrMagnitude;

                Vector3 dragacceleration = -object1.drag * vSquared;

                // Update Velocity on Accelleration

                accelerationThisFrame += dragacceleration;

                // Fg = Fpara + Fperp
                // Fperp = Fg projected onto normal

                object1.velocity += accelerationThisFrame * dt;
            }

            // Visualize
            //  Debug.DrawLine(prevPos, newPos, new Color(180.0f / 255.0f, 0.0f, 1.0f), 10);
            //  Debug.DrawLine(object1.transform.position, object1.transform.position + object1.velocity, Color.red);

            time += dt;
        }
    }

    private void CollisionUpdate()
    {
        for (int a = 0; a < physicsObjects.Count; a++)
        {
            PhysicsObject object1 = physicsObjects[a];

            for (int b = a + 1; b < physicsObjects.Count; b++)
            {
                bool isOverlapping = false;
                PhysicsObject object2 = physicsObjects[b];

                if (object1.GetType() == typeof(Sphere) && object2.GetType() == typeof(Sphere))
                {
                    isOverlapping = SphereSphereCollision(object1 as Sphere, object2 as Sphere);
                }
                else if (object1.GetType() == typeof(Sphere) && object2.GetType() == typeof(MyPlane))
                {
                    isOverlapping = SpherePlaneCollision(object1 as Sphere, object2 as MyPlane);
                }
                else if (object1.GetType() == typeof(MyPlane) && object2.GetType() == typeof(Sphere))
                {
                    isOverlapping = SpherePlaneCollision(object2 as Sphere, object1 as MyPlane);
                }
                if (isOverlapping)
                {
                    // Colliding

                    object1.GetComponent<Renderer>().material.color = Color.red;
                    object2.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }
    }

    public static bool SphereSphereCollision(Sphere ob1, Sphere ob2)
    {
        Vector3 Displacement = ob1.transform.position - ob2.transform.position;
        float distance = Displacement.magnitude;
        float overlap = (ob1.radius + ob2.radius) - distance;

        if (overlap > 0.0f)
        {
            Vector3 collisionNormal2to1 = Displacement / distance;
            Vector3 mtv = collisionNormal2to1 * overlap;
            ob1.transform.position += mtv * 0.5f;
            ob2.transform.position -= mtv * 0.5f;
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool SphereSphereOverlap(Sphere ob1, Sphere ob2)
    {
        float distance = (ob1.transform.position - ob2.transform.position).magnitude;
        return distance < (ob1.radius + ob2.radius);
    }

    public static bool SpherePlaneCollision(Sphere sphere, MyPlane plane)
    {
        Vector3 Displacement = sphere.transform.position - plane.transform.position;
        float positionAlongNormal = (plane.isHalspace ? Vector3.Dot(Displacement, plane.GetNormal()) : Mathf.Abs(Vector3.Dot(Displacement, plane.GetNormal())));
        float overlap = sphere.radius - positionAlongNormal;

        if (overlap > 0.0f)
        {
            sphere.FNormal = ((-Vector3.Dot(plane.GetNormal(), sphere.FGravity) * plane.GetNormal()));// + sphere.FNormal)/2;
            sphere.FFriction = -(sphere.FGravity + sphere.FNormal) * sphere.friction;

            Vector3 mtv = plane.GetNormal() * overlap;
            sphere.transform.position += mtv;
            return true;
        }
        else 
        { 
            return false; 
        }
    }

    public static bool SpherePlaneOverlap(Sphere sphere, MyPlane plane)
    {
        Vector3 planeToSphere = sphere.transform.position - plane.transform.position;
        float positionAlongNormal = Vector3.Dot(planeToSphere, plane.GetNormal());
        float distanceToPlane = Mathf.Abs(positionAlongNormal);
        if(plane.isHalspace)
        {
            return positionAlongNormal < sphere.radius;
        }
        return distanceToPlane < sphere.radius;
    }

    public void DrawForces(PhysicsObject physObject)
    {
        Debug.DrawLine(physObject.transform.position, physObject.transform.position + physObject.FNormal / 3, Color.green);
        Debug.DrawLine(physObject.transform.position, physObject.transform.position + physObject.FFriction / 3, new Color(1, 0.4f, 0));
        Debug.DrawLine(physObject.transform.position, physObject.transform.position + physObject.FGravity / 3, new Color(1, 0, 1));
    }
}
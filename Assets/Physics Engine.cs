using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class PhysicsEngine : MonoBehaviour
{
    public struct CollisionInfo
    {
        public bool isColliding;
        public Vector3 normal;

        public CollisionInfo(bool isColliding, Vector3 normal)
        {
            this.isColliding = isColliding; 
            this.normal = normal;
        }
    }

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

        CollisionUpdate();
        KinematicsUpdate();

        foreach (PhysicsObject obj in physicsObjects)
        {
            DrawForces(obj);
        }
    }


    private void KinematicsUpdate()
    {
        foreach (PhysicsObject obj in physicsObjects)
        {
            obj.FGravity = obj.mass* gravityAcceleration * obj.gravityScale;
            obj.FNet += obj.FGravity;

            if (!obj.isStatic)
            {
                // Update Velocity on Accelleration

                //  // Drag with forces
                //  Vector3 vSquared = obj.velocity * obj.velocity.sqrMagnitude;
                //  Vector3 dragForce = -obj.drag * vSquared;
                //  
                //  obj.FNet += obj.FGravity + dragForce;

                Vector3 accelerationThisFrame = obj.FNet / obj.mass;

                obj.velocity += accelerationThisFrame * dt;
               
                Vector3 momentum = obj.velocity * obj.mass;

                if (momentum.sqrMagnitude < 0.001f)
                {
                    obj.velocity = Vector3.zero;
                }

                Vector3 newPos = obj.transform.position + obj.velocity * dt;
                obj.transform.position = newPos;
            }
            else 
            { 
                obj.velocity = Vector3.zero; 
            }
            obj.FFriction = obj.FNet;
            obj.FNet = Vector3.zero;
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
                CollisionInfo collisionInfo = new CollisionInfo(false, Vector3.zero);
                PhysicsObject object2 = physicsObjects[b];

                if (object1.GetType() == typeof(Sphere) && object2.GetType() == typeof(Sphere))
                {
                    collisionInfo = SphereSphereCollision(object1 as Sphere, object2 as Sphere);
                }
                else if (object1.GetType() == typeof(Sphere) && object2.GetType() == typeof(MyPlane))
                {
                    collisionInfo = SpherePlaneCollision(object1 as Sphere, object2 as MyPlane);
                }
                else if (object1.GetType() == typeof(MyPlane) && object2.GetType() == typeof(Sphere))
                {
                    collisionInfo = SpherePlaneCollision(object2 as Sphere, object1 as MyPlane);
                }

                object1.FNormal = Vector3.zero;
                object2.FNormal = Vector3.zero;

                if (collisionInfo.isColliding)
                {
                    // Colliding
                    // Change Color to red
                    object1.GetComponent<Renderer>().material.color = Color.red;
                    object2.GetComponent<Renderer>().material.color = Color.red;

                    // Calculate the perpendicular conponent of gravity by vector projection of gravity onto the normal
                    float gravityDotNormal = Vector3.Dot(object2.FGravity, collisionInfo.normal);
                    Vector3 gravityProjectedNormal = collisionInfo.normal * gravityDotNormal;


                    // Calculate relative velocity to determine if we should apply kinetic friction or not
                    Vector3 vel1RelativeTo2 = object1.velocity - object2.velocity;
                    // Project relative velocity onto the surface
                    float velDotNormal = Vector3.Dot(vel1RelativeTo2, collisionInfo.normal);
                    Vector3 velProjectedNormal = collisionInfo.normal * velDotNormal; // part of velocity only aligned with normal axis

                    // Add the normal force that opposes gravity
                    if (gravityDotNormal < 0) // if normal and gravity in the same direction
                    {
                        object1.FNormal = gravityProjectedNormal;
                        object2.FNormal = -gravityProjectedNormal;

                        object1.FNet += object1.FNormal;
                        object2.FNet += object2.FNormal;

                        // Subtract the normally aligned velocity from the velocity, to get a 2D vector of the plane
                        Vector3 vel1RelativeTo2ProjectedOntoPlane = vel1RelativeTo2 - velProjectedNormal; // in-plane relative motion between 1 and 2

                        // Magnitude of friction is coefiicient of friction times normal force magnitude
                        if (vel1RelativeTo2ProjectedOntoPlane.sqrMagnitude > 0.00001f)
                        {
                            float coefficientOfFriction = Mathf.Clamp01(object1.friction * object2.friction);
                            float frictionMagnitude = object1.FNormal.magnitude * coefficientOfFriction;
                            object1.FFriction = -vel1RelativeTo2ProjectedOntoPlane.normalized * frictionMagnitude;
                            object2.FFriction = vel1RelativeTo2ProjectedOntoPlane.normalized * frictionMagnitude;

                            object1.FNet += object1.FFriction;
                            object2.FNet += object2.FFriction;
                        }
                    }


                    //  //  Lab 10 -- Bounciness/applying impulse from collision
                    //  if (velDotNormal < 0) // only if they're moving towards eachother to some degree
                    //  {
                    //      // Apply bounce
                    //      // Determine coefficient of restitution
                    //      float restitution;
                    //      if (velDotNormal > -0.5f) // Moving towards eachother, but not much
                    //      {
                    //          restitution = 0;
                    //      }
                    //      else
                    //      {
                    //          restitution = Mathf.Clamp01(object1.bounciness * object2.bounciness);
                    //      }
                    //      float deltaV = (1.0f + restitution) * velDotNormal;
                    //      // Notes say: Impulse = (1 + restitution) * Dot(v1Rel2, N) * m1 * m2 / (m1 + m2)
                    //      float impulse1D = deltaV * object1.mass * object2.mass / (object1.mass + object2.mass);
                    //      // Impulse is in the direction of the collisionNormal
                    //      Vector3 impulse3D = collisionInfo.normal * impulse1D;
                    //  
                    //  
                    //      Debug.DrawRay(object1.transform.position, impulse3D, Color.cyan, 0.2f, false);
                    //  
                    //      // Apply change in velocity based on impulse, in opposite directions for each obj
                    //      object1.velocity += -impulse3D / object1.mass;
                    //      object2.velocity += impulse3D / object2.mass;
                    //  
                    //  }
                }
            }
        }
    }

    public static CollisionInfo SphereSphereCollision(Sphere ob1, Sphere ob2)
    {
        Vector3 Displacement = ob1.transform.position - ob2.transform.position;
        float distance = Displacement.magnitude;
        float overlap = (ob1.radius + ob2.radius) - distance;

        if (overlap < 0.0f)
        {
            return new CollisionInfo(false, Vector3.zero);
        }

        Vector3 collisionNormal2to1;

        if (distance <= 0.00001f)
        {
            collisionNormal2to1 = Vector3.up;
        }
        else
        {
            collisionNormal2to1 = Displacement / distance;
        }

        Vector3 mtv = collisionNormal2to1 * overlap;
        ob1.transform.position += mtv * 0.5f;
        ob2.transform.position -= mtv * 0.5f;

        return new CollisionInfo(true, collisionNormal2to1);
    }

    public static bool SphereSphereOverlap(Sphere ob1, Sphere ob2)
    {
        float distance = (ob1.transform.position - ob2.transform.position).magnitude;
        return distance < (ob1.radius + ob2.radius);
    }

    public static CollisionInfo SpherePlaneCollision(Sphere sphere, MyPlane plane)
    {
        Vector3 Displacement = sphere.transform.position - plane.transform.position;
        float positionAlongNormal = (plane.isHalspace ? Vector3.Dot(Displacement, plane.GetNormal()) : Mathf.Abs(Vector3.Dot(Displacement, plane.GetNormal())));
        float overlap = sphere.radius - positionAlongNormal;

        if (overlap < 0.0f)
        {
            return new CollisionInfo(false, Vector3.zero);
        }

        //sphere.FNormal = ((-Vector3.Dot(plane.GetNormal(), sphere.FGravity) * plane.GetNormal()));// + sphere.FNormal)/2;
        //sphere.FFriction = -(sphere.FGravity + sphere.FNormal) * sphere.friction;

        Vector3 mtv = plane.GetNormal() * overlap;
        sphere.transform.position += mtv;
        return new CollisionInfo(true, plane.GetNormal());
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
        Debug.DrawRay(physObject.transform.position, physObject.velocity, Color.red);
        Debug.DrawRay(physObject.transform.position, physObject.FNormal, Color.green);
        Debug.DrawRay(physObject.transform.position, physObject.FFriction, Color.white/*new Color(1, 0.4f, 0)*/, 0.01f, false);
        Debug.DrawRay(physObject.transform.position, physObject.FGravity, new Color(1, 0, 1));
    }
}
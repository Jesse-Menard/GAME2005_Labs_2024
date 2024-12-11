using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

public class PhysicsEngine : MonoBehaviour
{
    // restitutionTable[Mat1, Mat2]
    float[,] restitutionTable =
        {
            {0.9f, 0.7f, 0.2f },
            {0.7f, 0.5f, 0.11f },
            {0.2f, 0.11f, 0.05f }
        };

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
    public Material[] materials = null;


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
            //UpdateMaterial(obj);

            if (obj.velEqRot)
            {
                obj.transform.rotation = Quaternion.LookRotation(obj.velocity, Vector3.up);
            }
            DrawForces(obj);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartSimulation();
        }
    }

    private void KinematicsUpdate()
    {
        foreach (PhysicsObject obj in physicsObjects)
        {
            if (!obj.isStatic)
            {
                obj.FGravity = obj.mass * gravityAcceleration * obj.gravityScale;
                obj.FNet += obj.FGravity;
                // Update Velocity on Accelleration

                //  // Drag with forces
                //  Vector3 vSquared = obj.velocity * obj.velocity.sqrMagnitude;
                //  Vector3 dragForce = -obj.drag * vSquared;
                //  
                //  obj.FNet += obj.FGravity + dragForce;

                Vector3 accelerationThisFrame = obj.FNet / obj.mass;

                obj.velocity += accelerationThisFrame * dt;
          
                Vector3 momentum = obj.velocity * obj.mass;
                // sleep
                if (momentum.sqrMagnitude < 0.05f)
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

            obj.FNet = Vector3.zero;
        }

        time += dt;
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
                    collisionInfo = SphereSphereCollision((Sphere)object1, object2 as Sphere);
                }
                else if (object1.GetType() == typeof(Sphere) && object2.GetType() == typeof(MyPlane))
                {
                    collisionInfo = SpherePlaneCollision((Sphere)object1, (MyPlane)object2);
                }
                else if (object1.GetType() == typeof(MyPlane) && object2.GetType() == typeof(Sphere))
                {
                    collisionInfo = SpherePlaneCollision((Sphere)object2, (MyPlane)object1);
                }
                else if (object1.GetType() == typeof(Boxx) && object2.GetType() == typeof(Boxx))
                {
                    collisionInfo = AABBCollision((Boxx)object1, (Boxx)object2);
                }
                else if (object1.GetType() == typeof(Sphere) && object2.GetType() == typeof(Boxx))
                {
                    collisionInfo = SphereAABBCollision((Sphere)object1, (Boxx)object2);
                }
                else if (object1.GetType() == typeof(Boxx) && object2.GetType() == typeof(Sphere))
                {
                    collisionInfo = SphereAABBCollision((Sphere)object2, (Boxx)object1);
                }

                if (collisionInfo.isColliding)
                {                   
                    // Colliding
                    // Change Color to red
                    if (object1.GetType() == typeof(Sphere) || object1.GetType() == typeof(Boxx))
                        object1.GetComponent<Renderer>().material.color = Color.red;
                    if (object2.GetType() == typeof(Sphere) || object2.GetType() == typeof(Boxx))
                        object2.GetComponent<Renderer>().material.color = Color.red;
                    
                    // Calculate the perpendicular conponent of gravity by vector projection of gravity onto the normal
                    float gravityDotNormal = Vector3.Dot(object1.FGravity != Vector3.zero ? object1.FGravity : object2.FGravity, collisionInfo.normal);
                    Vector3 gravityProjectedNormal = collisionInfo.normal * gravityDotNormal;
                    
                    
                    // Calculate relative velocity to determine if we should apply kinetic friction or not
                    Vector3 vel1RelativeTo2 = object1.velocity - object2.velocity;
                    // Project relative velocity onto the surface
                    float velDotNormal = Vector3.Dot(vel1RelativeTo2, collisionInfo.normal);
                    Vector3 velProjectedNormal = collisionInfo.normal * velDotNormal; // part of velocity only aligned with normal axis
                    
                    // Add the normal force that opposes gravity
                    if (gravityDotNormal < 0) // if normal and gravity in the same direction
                    {
                        //if (object1.isStatic || object2.isStatic)
                        //{
                        //    object1.FNormal = gravityProjectedNormal;
                        //    object2.FNormal = -gravityProjectedNormal;
                        //}
                        //else
                        //{
                            object1.FNormal = -gravityProjectedNormal;
                            object2.FNormal = gravityProjectedNormal;
                        //}
                               
                        object1.FNet += object1.FNormal;
                        object2.FNet += object2.FNormal;
                    
                        // Subtract the normally aligned velocity from the velocity, to get a 2D vector of the plane
                        Vector3 vel1RelativeTo2ProjectedOntoPlane = vel1RelativeTo2 - velProjectedNormal; // in-plane relative motion between 1 and 2
                    
                        // Magnitude of friction is coefiicient of friction times normal force magnitude
                        if (vel1RelativeTo2ProjectedOntoPlane.sqrMagnitude > 0.0001f)
                        {
                            float coefficientOfFriction = Mathf.Clamp01(object1.friction * object2.friction);
                            float frictionMagnitude = object1.FNormal.magnitude * coefficientOfFriction;
                            object1.FFriction = -vel1RelativeTo2ProjectedOntoPlane.normalized * frictionMagnitude;
                            object2.FFriction = vel1RelativeTo2ProjectedOntoPlane.normalized * frictionMagnitude;
                        
                            object1.FNet += object1.FFriction;
                            object2.FNet += object2.FFriction;
                        }
                        //  if (object1.GetType() == typeof(Boxx) && object2.GetType() == typeof(Boxx) && vel1RelativeTo2ProjectedOntoPlane.sqrMagnitude == 0)
                        //  {
                        //      float coefficientOfFriction = Mathf.Clamp01(object1.friction * object2.friction);
                        //      float frictionMagnitude = object1.FNormal.magnitude * coefficientOfFriction;
                        //  
                        //      Vector3 velocityDifference2to1 = object1.velocity - object2.velocity;
                        //  
                        //      object1.FFriction = -velocityDifference2to1.normalized * frictionMagnitude * 100;
                        //      object2.FFriction = velocityDifference2to1.normalized * frictionMagnitude * 100;
                        //  
                        //      object1.FNet += object1.FFriction;
                        //      object2.FNet += object2.FFriction;
                        //  }
                    }

                    //  Lab 10 -- Bounciness/applying impulse from collision
                    if (velDotNormal < 0) // only if they're moving towards eachother to some degree
                    {
                        // Apply bounce
                        // Determine coefficient of restitution
                        float restitution = restitutionTable[(int)object1.material, (int)object2.material];

                        if (velDotNormal > -0.5f) // Moving towards eachother, but not much
                        {
                            restitution = 0;
                        }
                        //  else
                        //  {
                        //      restitution = Mathf.Clamp01(object1.bounciness * object2.bounciness);
                        //  }

                        float deltaV = (1.0f + restitution) * velDotNormal;
                        // Notes say: Impulse = (1 + restitution) * Dot(v1Rel2, N) * m1 * m2 / (m1 + m2)
                        float impulse1D = deltaV * object1.mass * object2.mass / (object1.mass + object2.mass);
                        // Impulse is in the direction of the collisionNormal
                        Vector3 impulse3D = collisionInfo.normal * impulse1D;
                                        
                        Debug.DrawRay(object1.transform.position, impulse3D, Color.cyan, 1.0f, false);
                        Debug.DrawRay(object2.transform.position, impulse3D, Color.cyan, 1.0f, false);
                    
                        // Apply change in velocity based on impulse, in opposite directions for each obj
                        object1.velocity -= impulse3D / object1.mass;
                        object2.velocity += impulse3D / object2.mass;
                    }
                }
            }
        }
    }

    public static CollisionInfo SphereSphereCollision(Sphere ob1, Sphere ob2)
    {
        Vector3 Displacement2to1 = ob1.transform.position - ob2.transform.position;
        float distance = Displacement2to1.magnitude;
        float overlap = ob1.radius + ob2.radius - distance;

        if (overlap <= 0.0f)
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
            collisionNormal2to1 = Displacement2to1 / distance;
        }

        Vector3 mtv = collisionNormal2to1 * overlap;
        if (ob1.isStatic && ob2.isStatic)
        {
            return new CollisionInfo(true, collisionNormal2to1);
        }
        else if (ob1.isStatic && !ob2.isStatic)
        {
            ob2.transform.position -= mtv;
        }
        else if (ob2.isStatic && !ob1.isStatic)
        {
            ob1.transform.position += mtv;
        }
        else
        {
            ob1.transform.position += mtv * 0.5f;
            ob2.transform.position -= mtv * 0.5f;
        }

        return new CollisionInfo(true, collisionNormal2to1);
    }

    public static CollisionInfo SpherePlaneCollision(Sphere sphere, MyPlane plane)
    {
        Vector3 Displacement = sphere.transform.position - plane.transform.position;
        float positionAlongNormal = Vector3.Dot(Displacement, plane.GetNormal());

        float distanceToPlane;

        if (plane.isHalspace) 
        {
            distanceToPlane = positionAlongNormal;  
        }
        else
        {
            distanceToPlane = Mathf.Abs(positionAlongNormal);
        }
        

        float overlap = sphere.radius - distanceToPlane;

        if (overlap < 0)
        {
            return new CollisionInfo(false, Vector3.zero);
        }

        Vector3 mtv = plane.GetNormal() * overlap;
        if (!sphere.isStatic)
            sphere.transform.position += mtv;
        return new CollisionInfo(true, plane.GetNormal());
    }

    public static CollisionInfo AABBCollision(Boxx box1, Boxx box2)
    {
        // ---- Box 1 ----- Width
        if (box1.transform.position.x - box1.width <= box2.transform.position.x + box2.width &&
            box1.transform.position.x + box1.width >= box2.transform.position.x - box2.width &&
        // Height
            box1.transform.position.y - box1.height < box2.transform.position.y + box2.height &&
            box1.transform.position.y + box1.height >= box2.transform.position.y - box2.height &&
            // Length
            box1.transform.position.z - box1.length < box2.transform.position.z + box2.length &&
            box1.transform.position.z + box1.length >= box2.transform.position.z - box2.length)
        {
            Vector3 displacement2to1 = box1.transform.position - box2.transform.position;
            float distance = displacement2to1.magnitude;

            float overlapX = Mathf.Abs(displacement2to1.x) - box1.width - box2.width;
            float overlapY = Mathf.Abs(displacement2to1.y) - box1.height - box2.height;
            float overlapZ = Mathf.Abs(displacement2to1.z) - box1.length - box2.length;

            Vector3 mtv = Vector3.zero;

            if (overlapX < 0 && overlapX >= overlapY && overlapX >= overlapZ)
            {
                mtv.x = displacement2to1.x > 0 ? -overlapX : overlapX;
            }
            if (overlapY < 0 && overlapY > overlapX && overlapY > overlapZ)
            {
                mtv.y = displacement2to1.y > 0 ? -overlapY : overlapY;
            }
            if (overlapZ < 0 && overlapZ > overlapX && overlapZ > overlapY)
            {
                mtv.z = displacement2to1.z > 0 ? -overlapZ : overlapZ;
            }


            if (box1.isStatic && box2.isStatic)
            {
                return new CollisionInfo(true, mtv.normalized);
            }
            else if (box1.isStatic && !box2.isStatic)
            {
                box2.transform.position -= mtv;
            }
            else if (box2.isStatic && !box1.isStatic)
            {
                box1.transform.position += mtv;
            }
            else
            {
                box1.transform.position += mtv * 0.5f;
                box2.transform.position -= mtv * 0.5f;
            }

            if(mtv.x != 0)
            {
                mtv.x = displacement2to1.x > 0 ? -Mathf.Sign(overlapX) : Mathf.Sign(overlapX);
            }
            else if(mtv.y != 0)
            {
                mtv.y = displacement2to1.y > 0 ? -Mathf.Sign(overlapY) : Mathf.Sign(overlapY);
            }
            else if(mtv.z != 0)
            {
                mtv.z = displacement2to1.z > 0 ? -Mathf.Sign(overlapZ) : Mathf.Sign(overlapZ);
            }

            return new CollisionInfo(true, mtv.normalized);
        }

        return new CollisionInfo(false, Vector3.zero);
    }

    public static CollisionInfo SphereAABBCollision(Sphere sphere, Boxx box)
    {
        Vector3 clampedBoxPosition = new Vector3
        (
            Mathf.Clamp(sphere.transform.position.x, box.transform.position.x - box.width, box.transform.position.x + box.width),
            Mathf.Clamp(sphere.transform.position.y, box.transform.position.y - box.height, box.transform.position.y + box.height),
            Mathf.Clamp(sphere.transform.position.z, box.transform.position.z - box.length, box.transform.position.z + box.length)
        );

        Vector3 displacementStoB = sphere.transform.position - clampedBoxPosition;
        float distance = displacementStoB.magnitude;
        float overlap = sphere.radius - distance;

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
            collisionNormal2to1 = displacementStoB / distance;
        }

        Vector3 mtv = collisionNormal2to1 * overlap;
        if (sphere.isStatic && box.isStatic)
        {
            return new CollisionInfo(true, collisionNormal2to1);
        }
        else if (sphere.isStatic && !box.isStatic)
        {
            box.transform.position -= mtv;
        }
        else if (box.isStatic && !sphere.isStatic)
        {
            sphere.transform.position += mtv;
        }
        else
        {
            sphere.transform.position += mtv * 0.5f;
            box.transform.position -= mtv * 0.5f;
        }

        return new CollisionInfo(true, collisionNormal2to1);
    }

    public void DrawForces(PhysicsObject physObject)
    {
        Debug.DrawRay(physObject.transform.position, physObject.velocity, Color.red);
        Debug.DrawRay(physObject.transform.position, physObject.FNormal, Color.green);
        Debug.DrawRay(physObject.transform.position, physObject.FFriction, new Color(1, 0.4f, 0));
        Debug.DrawRay(physObject.transform.position, physObject.FGravity, new Color(1, 0, 1));
    }

    public void RestartSimulation()
    {
        foreach (PhysicsObject obj in physicsObjects)
        {
            obj.velocity = obj.initialVelocity; 
            obj.transform.position = obj.initialPosition; 
        }
    }

    public void UpdateMaterial(PhysicsObject obj)
    {
        if (obj.GetComponent<Renderer>().material != materials[(int)obj.material])
        {
            obj.GetComponent<Renderer>().material = materials[(int)obj.material];
        }
    }
}
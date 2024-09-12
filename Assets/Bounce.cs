using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    // public float Xpos, Ypos = 0;
    public float mass = 1.0f;
    public float gravity = -1.0f;
    public float friction = 0.98f;

    public Vector3 Vel = new Vector3(0.0f,0.0f,0.0f);
    float time = 0;
    
    private void FixedUpdate()
    {
        float deltaT = 1.0f / 60.0f;

        if (friction > 1.0f) {friction = 1.0f} // gets funky when over 1

        // if/else prevents passively lowering position when reversing vel
        if (transform.position.y <= 0)
        {
            Vel *= -1.0f;
        }
        else
        {
            Vel.y += mass * gravity * deltaT;
            Vel *= friction;
        }

        // Prevent infinite bounce
        if (transform.position.y <= mass * -gravity * deltaT * friction && 
            Vel.y <= mass * -gravity * deltaT * friction)
        {
            Vel.y *= 0.75f;
        }

        transform.position += Vel;
        
        time += deltaT;
    }
}

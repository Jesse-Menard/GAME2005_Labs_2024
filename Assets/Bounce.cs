using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float mass = 1.0f;
    public float gravity = -1.0f;
    public float friction = 0.98f;

    public Vector3 vel = new Vector3(0.0f, 0.0f, 0.0f);
    float time = 0;
    bool applyFric = false;
    
    private void FixedUpdate()
    {
        float deltaT = 1.0f / 60.0f;

        // Prevent dampen when low to prevent infinite
        if (transform.position.y <= mass * -gravity * deltaT && 
            vel.magnitude <= mass * -gravity * deltaT * 2.0f)
        {
            vel *= 0.50f;
            if (transform.position.y < 0.0f) 
            {
                transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z) ;
            }
        }
        // If next change would make neg, flip instead
        else if (transform.position.y + vel.y < 0.0f)
        {
            transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
            vel.y *= -1.0f;
            applyFric = true;
        }         
        // gravity & friction
        else
        {            
            vel.y += mass * gravity * deltaT;
           
            if (applyFric)
            {
                vel *= friction;
                applyFric = false;
            }
        }

        transform.position += vel;
        
        time += deltaT;
    }
}

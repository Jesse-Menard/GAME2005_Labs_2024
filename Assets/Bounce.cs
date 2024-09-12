using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    // public float Xpos, Ypos = 0;
    public float mass = 1.0f;
    public float gravity = -1.0f;

    Vector3 Vel = new Vector3();
    float time = 0;
    
    private void FixedUpdate()
    {
        float deltaT = 1.0f / 60.0f;



        if (transform.position.y <= 0)
        {
            Vel *= -1.0f;
        }
        else
        {
            Vel.y += mass * gravity * deltaT;
        }
        transform.position += Vel;
        
        time += deltaT;
    }
}

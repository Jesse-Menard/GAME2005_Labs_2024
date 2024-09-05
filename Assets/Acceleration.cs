using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acceleration : MonoBehaviour
{
    public float Xpos, Ypos = 0;
    public float accel = 1.0f;
    public float amp = 3.0f;
    public float time = 0;

    private void FixedUpdate()
    {
        float deltaT = 1.0f / 60.0f;

        Xpos += (-Mathf.Sin(time * accel) * accel * amp * deltaT);
        Ypos += (-Mathf.Cos(time * accel) * accel * amp * deltaT);

        transform.position = new Vector3(Xpos, Ypos, transform.position.z);

        time += deltaT;
    }
}

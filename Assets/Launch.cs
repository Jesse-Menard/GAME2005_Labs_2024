
using System;
using UnityEngine;

public class Launch : MonoBehaviour
{
    public float angle;
    public float speed;
    public float startHeight;

    Bounce b;

    float deltaT = 1.0f / 60.0f;

    private void Start()
    {
        b = GetComponent<Bounce>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Launch
            b.vel = new(Mathf.Cos(angle / Mathf.Rad2Deg) * speed * deltaT, Mathf.Sin(angle / Mathf.Rad2Deg) * speed * deltaT, 0.0f);
            transform.position = new(0.0f, startHeight, 0.0f);
        }
    }

    private void FixedUpdate()
    {
        // Show lauch angle w/ vel
        Debug.DrawLine(new Vector3(0.0f, startHeight, 0.0f), new Vector3(Mathf.Cos(angle / Mathf.Rad2Deg) * speed * 0.2f, Mathf.Sin(angle / Mathf.Rad2Deg) * speed * 0.2f + startHeight, 0.0f), Color.blue);        
    }
}

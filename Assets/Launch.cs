
using System;
using UnityEngine;

public class Launch : MonoBehaviour
{
    public float angle;
    public float speed;
    public float startHeight;

    float deltaT = 1.0f / 60.0f;
    public PhysicsObject ProjectileObject;
    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Launch
            PhysicsObject launchObject = Instantiate(ProjectileObject);
            PhysicsObject physicsObject = launchObject.GetComponent<PhysicsObject>();

            physicsObject.velocity = new(Mathf.Cos(angle / Mathf.Rad2Deg) * speed, Mathf.Sin(angle / Mathf.Rad2Deg) * speed, 0.0f);
            physicsObject.transform.position = new(0.0f, startHeight, 0.0f);
            physicsObject.FNet = Vector3.zero;
            physicsObject.FNormal = Vector3.zero;
            physicsObject.FGravity = Vector3.zero;
            physicsObject.FFriction = Vector3.zero;
            physicsObject.material = surfaceMaterial.CLOTH;
            physicsObject.mass = 2;
            physicsObject.friction = 1.0f;
        }
    }

    private void FixedUpdate()
    {
        // Show lauch angle w/ vel
        Debug.DrawLine(new Vector3(0.0f, startHeight, 0.0f), new Vector3(Mathf.Cos(angle / Mathf.Rad2Deg) * speed * 0.2f, Mathf.Sin(angle / Mathf.Rad2Deg) * speed * 0.2f + startHeight, 0.0f), Color.blue);        
    }
}

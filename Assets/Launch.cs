
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Launch : MonoBehaviour
{
    public float MaxForce;

    float deltaT = 1.0f / 60.0f;
    public PhysicsObject ProjectileObjectSphere;
    public PhysicsObject ProjectileObjectBox;

    public bool useSphere = true;

    public Vector3 startingMousePos = Vector3.zero;
    public Vector3 deltaMousePos = Vector3.zero;
    public Vector3 position = Vector3.zero;

    PhysicsObject createdObjectReference;

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))            
        {
            PhysicsObject launchObject = Instantiate(useSphere ? ProjectileObjectSphere : ProjectileObjectBox);
            createdObjectReference = launchObject.GetComponent<PhysicsObject>();
            startingMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {

            deltaMousePos = startingMousePos - Input.mousePosition;
            //if (deltaMousePos.magnitude > 20)
            {
                deltaMousePos = deltaMousePos.normalized * Mathf.Clamp(deltaMousePos.magnitude/15, 0.0f, 3.0f);
            }
            createdObjectReference.transform.position = -deltaMousePos;
        }

        if (Input.GetMouseButtonUp(0) && startingMousePos.magnitude != 0)
        {


            Release();
            startingMousePos = Vector3.zero;
            deltaMousePos = Vector3.zero;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            useSphere = !useSphere;
        }

    }

    private void FixedUpdate()
    {
        // Show lauch angle w/ vel
        Debug.DrawLine(-deltaMousePos, transform.position, Color.blue);        
    }

    public void Release()
    {
        if(deltaMousePos.magnitude == 0)
        {
            deltaMousePos = Vector3.right;
        }

        createdObjectReference.velocity = (deltaMousePos * MaxForce) / createdObjectReference.mass;
    }
}

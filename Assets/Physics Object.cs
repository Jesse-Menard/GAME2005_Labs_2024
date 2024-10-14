using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public float radius = 1;
    public float mass = 1;
    [Range(0f, 2f)]
    public float drag = 0.1f;
    public Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        PhysicsEngine.Instance.physicsObjects.Add(this);
    }

    private void OnValidate() // Editor only
    {
        transform.localScale = new Vector3(radius, radius, radius) * 2f;
    }

    private void Update() // Editor only
    {
        transform.localScale = new Vector3(radius, radius, radius) * 2f;
    }



}
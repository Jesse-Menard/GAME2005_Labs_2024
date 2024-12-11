using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boxx : PhysicsObject
{
    [Range(0.001f, 100)]
    public float width = 1;
    [Range(0.001f, 20)]
    public float height = 1;
    [Range(0.001f, 100)]
    public float length = 1;

    // Start is called before the first frame update
    private void OnValidate() // Editor only
    {
        transform.localScale = new Vector3(width, height, length) * 2f;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.localScale = new Vector3(width, height, length) * 2f;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lines : MonoBehaviour
{
    float lineLength = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + new Vector3(lineLength, 0, 0), new Color(1,0,0));
        Debug.DrawLine(transform.position, transform.position + new Vector3(0, lineLength, 0), new Color(0,1,0));
        Debug.DrawLine(transform.position, transform.position + new Vector3(0, 0, lineLength), new Color(0,0,1));
    }
}

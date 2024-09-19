
using UnityEngine;

public class Launch : MonoBehaviour
{
    public float angle;
    public float speed;
    public float startHeight = 1;

    new Vector3 velocity; // = new (Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed, 0.0f);

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
            // velocity = new(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed, 0.0f);
            b.vel = new(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed, 0.0f);
            transform.position = new(0.0f, startHeight, 0.0f);
            //Debug.DrawLine(transform.position, transform.position + velocity, Color.red, 2);
        }
    }

    private void FixedUpdate()
    {
        Debug.DrawLine(new Vector3(0.0f, startHeight, 0.0f), new Vector3(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed + startHeight, 0.0f), Color.blue);

        //transform.position += b.vel * deltaT;
    }
}

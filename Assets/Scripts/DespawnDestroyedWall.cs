using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnDestroyedWall : MonoBehaviour
{
    public float secondsToLive = 3.0f;
    public float torque = 2000.0f;
    public float force = 2000.0f;
    private float age = 0;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.AddTorque(Random.Range(-torque, torque));
        rigidbody.AddForce(new Vector2(Random.Range(-force, force), Random.Range(0.0f, force)));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        age += Time.fixedDeltaTime;
        if (age >= secondsToLive) {
            Destroy(gameObject);
        }
    }
}

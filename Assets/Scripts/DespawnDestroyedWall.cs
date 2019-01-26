using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnDestroyedWall : MonoBehaviour
{
    public float secondsToLive = 10.0f;
    public float torque = 5.0f;
    public float force = 1e-10f;
    private float age = 0;
    private float realSecondsToLive = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);
        rigidbody.AddForce(new Vector2(Random.Range(-force, force), Random.Range(0.0f, force)), ForceMode2D.Impulse);

        realSecondsToLive = Random.Range(secondsToLive/ 2, secondsToLive * 2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        age += Time.fixedDeltaTime;
        if (age >= realSecondsToLive) {
            Destroy(gameObject);
        }
    }
}

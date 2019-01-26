using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnDestroyedWall : MonoBehaviour
{
    public float secondsToLive = 10.0f;
    public float torque = 5.0f;
    public float force = 1e-10f;
    public bool persist = false;
    private float age = 0;
    private float realSecondsToLive = 0.0f;

    // Start is called before the first frame update
    void OnEnable()
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.AddTorque(Random.Range(-torque, torque), ForceMode2D.Impulse);
        rigidbody.AddForce(new Vector2(Random.Range(-force, force), Random.Range(0.0f, force)), ForceMode2D.Impulse);

        realSecondsToLive = Random.Range(secondsToLive/ 2, secondsToLive * 2);
        age = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        age += Time.fixedDeltaTime;
        if (age >= realSecondsToLive) {
            if (persist)
            {
                // Hide self when life time is up. Player will reuse
                gameObject.SetActive(false);
            } else {
                Destroy(gameObject);
            }
        }
    }
}

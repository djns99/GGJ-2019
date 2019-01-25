using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    bool active = true;
    Vector2 pos;
    public GameObject bullet;
    GameObject grabber;
    // Start is called before the first frame update
    void Start()
    {
        pos.x = 0;
        pos.y = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(active)
        {
            Shoot();
        }
        else
        {
            pos = grabber.transform.localPosition;
        }
    }

    private void Shoot()
    {
        float angle = Random.value;

        Instantiate(bullet, pos, Quaternion.identity);
    }

    public void Grab(GameObject grabbed)
    {
        grabber = grabbed;
        active = false;
    }

    public void Release()
    {
        active = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPivot : MonoBehaviour
{
    private float rot = 0.0f;
    public float speed = 1.0f;
    public float minAngle = 0.0f;
    public float maxAngle = 90.0f;


    // Update is called once per frame
    void Update()
    {
        rot += speed * Time.deltaTime;
        float angle = ( Mathf.Sin(rot) + 1 ) / 2;

        transform.eulerAngles = new Vector3(0, 0, angle * (maxAngle - minAngle) + minAngle);
    }
}

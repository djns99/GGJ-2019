using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentrePlayer : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = player.transform.localPosition;
        pos.z = -100.0f;
        transform.localPosition = pos;
    }
}

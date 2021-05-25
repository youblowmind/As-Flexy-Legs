using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform camTarget;
    public float length;
    public float heigth;
    public float distance;
    public float smoothness;

    Vector3 velocity;
    Vector3 pos = Vector3.zero;

    void Start()
    {
        pos.x = transform.position.x;
        pos.y = transform.position.y;
        pos.z = transform.position.z;
    }

    void FixedUpdate()
    {
        if (!camTarget)
            return;

        pos.x = camTarget.transform.position.x + length;
        pos.z = camTarget.transform.position.z + distance;
        pos.y = camTarget.transform.position.y + heigth;

        /*if (camTarget.position.z > 3.0f)
        {
            pos.z = camTarget.transform.position.z - distance;
        }*/

        transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, smoothness);
    }
}

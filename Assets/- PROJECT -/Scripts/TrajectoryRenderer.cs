using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(LineRenderer))]
public class TrajectoryRenderer : MonoBehaviour
{
    public Vector3 initalVelocity;
    LineRenderer lineRenderer;
    Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void FixedUpdate() {
        if (Input.GetMouseButtonDown(0))
        {

            //Time of flight calculation

            float t;
            t = (-1f * initalVelocity.y) / Physics.gravity.y;
            t = 2f * t;

            //Trajectory calculation

            lineRenderer.positionCount = 100;
            Vector3 trajectoryPoint;

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                //
                float time = t * i / (float)(lineRenderer.positionCount);
                trajectoryPoint = transform.position + initalVelocity * time + 0.5f * Physics.gravity * time * time;
                lineRenderer.SetPosition(i, trajectoryPoint);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            rb.velocity = initalVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        lineRenderer.positionCount = 0;
    }
}
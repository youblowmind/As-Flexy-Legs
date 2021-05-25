using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeMachineMovement : MonoBehaviour
{
    [HideInInspector] public float speed = 0;
    [SerializeField] private PlayerController playerScript;

    void FixedUpdate() {
        Movement();
    }

    private void Movement() {
        if (transform.position.z < playerScript.transform.position.z)
            transform.position = new Vector3(transform.position.x, transform.position.y, playerScript.transform.position.z);
        else
            transform.Translate(transform.forward * speed * Time.deltaTime);
    }
}

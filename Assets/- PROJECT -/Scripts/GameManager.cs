using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [SerializeField] private RopeMachineMovement ropeMach;
    [SerializeField] private PlayerController playerScript;
    [SerializeField] private float playerAndWheelSpeed;

    void Awake() {
        instance = this;
    }

    public void StartGame() {
        playerScript.speed = playerAndWheelSpeed;
        ropeMach.speed = playerAndWheelSpeed;
    }
}

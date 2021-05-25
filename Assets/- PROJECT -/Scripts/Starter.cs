using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starter : MonoBehaviour
{
    void Update(){
        if (Input.GetMouseButtonDown(0))
        {
            GameManager.instance.StartGame();
            Destroy(this);
        }
    }
}

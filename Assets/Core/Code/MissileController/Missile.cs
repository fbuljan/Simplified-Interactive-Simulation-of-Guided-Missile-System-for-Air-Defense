using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Plane>() != null)
        {
            print("Trigger Enter Plane");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Plane>() != null)
        {
            print("Trigger Exit  Plane");
        }
    }

}
    


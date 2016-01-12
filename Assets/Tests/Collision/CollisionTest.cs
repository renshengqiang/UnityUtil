using UnityEngine;
using System.Collections;
using Log;

public class CollisionTest : MonoBehaviour {
    public int id = 1;
    void OnTriggerEnter( Collider other )
    {
        Logger.Log("OnTriggerEnter: " + other + " " + id);
    } 

    void OnCollisionEnter( Collision collisionInfo )
    {
        Logger.Log("OnCollisionEnter: " + collisionInfo + " " + id);
    }
}

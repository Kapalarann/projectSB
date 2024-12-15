using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallReset : MonoBehaviour
{
    public Transform spawnPoint;
    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.name + " fell off the map!");
        if(collision.gameObject.GetComponent<CharacterController>() != null)
        {
            collision.gameObject.GetComponent<CharacterController>().enabled = false;
            collision.gameObject.transform.position = spawnPoint.position;
            collision.gameObject.GetComponent<CharacterController>().enabled = true;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBomb : MonoBehaviour
{
    // Start is called before the first frame update
    public static event Action Explode;
   
    void Start()
    {
        Destroy(gameObject, 10f);
        FindObjectOfType<Camera>().GetComponent<CameraControl>().SetFireBall(gameObject);
        //follow me
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //deal damage
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //deal damage
        }
        Destroy(gameObject);
    }
    private void OnDestroy()
    {//if not in tutorial
        //maybe put all of this inside a coroutine
        if (!gameObject.CompareTag("MyProjectile"))
        {
            //start moving towards me
            Explode?.Invoke();
        }
        //Stop following me
        //start moving towards enemy
    }
}

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
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //deal damage
        }
        Destroy(gameObject);
    }
   
    private void OnDestroy()
    {//if not in tutorial
        Explode?.Invoke();
    }
}

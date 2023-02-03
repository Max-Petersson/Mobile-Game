using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] List <GameObject> players = new List<GameObject>();
    float airTime = 0f;
    GameObject firebomb = null;
    Camera mainCam;
    float cameraSize;
    Rigidbody2D body;
    Vector3 velocity = Vector3.zero;
    float smoothTime = 0.3f;
    float timer;
    private void Start()
    {
        mainCam = GetComponent<Camera>();
        cameraSize = mainCam.orthographicSize;
    }
    // Update is called once per frame
    void Update()
    {
        
        if (firebomb != null)
        {
            if(body == null)
            {
                body = firebomb.GetComponent<Rigidbody2D>();
            }
            velocity = body.velocity;
            transform.position = Vector3.SmoothDamp(transform.position, firebomb.gameObject.transform.position, ref velocity , smoothTime);
            
            if (mainCam.orthographicSize <= 30)
            {
                mainCam.orthographicSize += 5f * Time.deltaTime;
                
            }
                
        }
    }
    public void SetFireBall(GameObject fireball)
    {
        firebomb = fireball;
    }
    public void StartMoving(bool moveToPlayer1)
    {
        if (moveToPlayer1)
        {
            if(FirebaseTest.mySpot == "Player1")
            {
                //move towards player2
            }
            else
            {
                //move towards player1
            }
            // lerp towards enemy
        }
        else
        {
            if(FirebaseTest.mySpot == "Player1")
            {
                //move towards player 2
            }
            else
            {
                //move towards player1;
            }
            //lerp towards 
        }
    }
}

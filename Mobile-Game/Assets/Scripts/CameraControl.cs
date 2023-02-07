using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] List <GameObject> players = new List<GameObject>();
    
    GameObject firebomb = null;
    Camera mainCam;
    float originalCamSize;
    Rigidbody2D body;
    Vector3 velocity = Vector3.zero;
    float smoothTime = 0.3f;
    float timer;
    int playerToFollow = 0;
    private bool shouldSwitch;

    private void Start()
    {
        mainCam = GetComponent<Camera>();
        originalCamSize = mainCam.orthographicSize;
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
                mainCam.orthographicSize = Mathf.MoveTowards(mainCam.orthographicSize, 30f, 5 * Time.deltaTime);
                
            }
                
        }
        if(firebomb == null && shouldSwitch == true)
        {
            mainCam.orthographicSize = originalCamSize;
            StartMoving();
            shouldSwitch = false;
        }
    }
    public void SetFireBall(GameObject fireball)
    {
        firebomb = fireball;
        shouldSwitch = true;
        //maybe introduce a bool to start update methods
    }
    public void StartMoving(bool moveToPlayer1 = true)
    {
        if (playerToFollow == 0)
        {
            playerToFollow++;
        }
        else
        {
            playerToFollow--;
        }
        gameObject.transform.position = players[playerToFollow].transform.position + new Vector3(0, 0, gameObject.transform.position.z);
        
       

        //if (moveToPlayer1)
        //{
        //    if(FirebaseTest.mySpot == "Player1")
        //    {
        //        //move towards player2
        //    }
        //    else
        //    {
        //        //move towards player1
        //    }
        //    // lerp towards enemy
        //}
        //else
        //{
        //    if(FirebaseTest.mySpot == "Player1")
        //    {
        //        //move towards player 2
        //    }
        //    else
        //    {
        //        //move towards player1;
        //    }
        //    //lerp towards 
        //}
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //player related
    [SerializeField] List <GameObject> players = new List<GameObject>();
    int playerToFollow = 0;
    private bool shouldSwitch;

    //firebomb related
    GameObject firebomb = null;
    Rigidbody2D body;

    //CameraRelated
    Camera mainCam;
    float originalCamSize;
    float smoothTime = 0.3f;
    Vector3 camerazPos = new Vector3(0, 0, -5f);
    Vector3 velocity = Vector3.zero;


    private void Start()
    {
        mainCam = GetComponent<Camera>();
        originalCamSize = mainCam.orthographicSize;
    }
    void Update()
    {
        
        if (firebomb != null) // if there is a firebomb follow the firebomb
        {
            if(body == null)
            {
                body = firebomb.GetComponent<Rigidbody2D>();
            }
            velocity = body.velocity;
            transform.position = Vector3.SmoothDamp(transform.position, firebomb.gameObject.transform.position, ref velocity , smoothTime);
            
            if (mainCam.orthographicSize <= 30)
            {
                mainCam.orthographicSize = Mathf.MoveTowards(mainCam.orthographicSize, 30f, 10 * Time.deltaTime);
            }
        }

        if(firebomb == null && shouldSwitch == true) // if there is no firebomb switch the player focus
        {
            StartMoving();
            shouldSwitch = false;
        }
        else if(firebomb == null) // then move towards that player
        {
            mainCam.orthographicSize = Mathf.MoveTowards(mainCam.orthographicSize, originalCamSize, 5 * Time.deltaTime);

            float x = Mathf.MoveTowards(transform.position.x , players[playerToFollow].transform.position.x, 10 * Time.deltaTime);
            float y = Mathf.MoveTowards(transform.position.y, players[playerToFollow].transform.position.y, 10 * Time.deltaTime);
            Vector3 pos = new Vector3(x, 0, camerazPos.z);

            transform.position = Vector3.MoveTowards(transform.position, pos, 10 * Time.deltaTime);
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
            playerToFollow = 1;
        }
        else
        {
            playerToFollow = 0;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public List <GameObject> characters = new List<GameObject>();
    [SerializeField]private GameObject mainCamera = null;
    private void Start()
    {
        foreach(Transform t in characters[0].transform)
        {
            Debug.Log(t.name);
            Debug.Log(FirebaseTest.mySpot);
        }
        SpawnPlayers();
        SetCamera();
    }

    private void SetCamera()
    {
        if (FirebaseTest.mySpot == "Player1")
        {
            mainCamera.transform.position = new Vector3 (characters[0].transform.position.x, characters[0].transform.position.y, mainCamera.transform.position.z);
            //camera should be at player 1
        }
        else
        {
            mainCamera.transform.position = new Vector3(characters[1].transform.position.x, characters[1].transform.position.y, mainCamera.transform.position.z);
            //camera should be at player 2
        }
    }

   
    private void SpawnPlayers() //every script on the characters must be disabled
    {
        if(FirebaseTest.mySpot == "Player1") // if im player1 set up from player1 perspective
        {
            SetUp(characters[0], true);
            SetUp(characters[1], false);
        }
        else //if im player2 set up from player 2 perspective
        {
            SetUp(characters[1], true);
            SetUp(characters[0], false);
        }
    }
    private void SetUp(GameObject player, bool shouldBeActive)
    {
        if(shouldBeActive == true)
        {
            player.GetComponent<ThrowProjectile>().enabled = shouldBeActive;
            foreach (Transform child in player.transform)
            {
                if (child.CompareTag("RotationPoint"))
                {
                    child.GetComponent<Aim>().enabled = shouldBeActive;
                    Debug.Log(shouldBeActive);
                }
            }
        }
        else
        {
            foreach(Transform child in player.transform)
            {
                if (child.CompareTag("RotationPoint"))
                {
                    child.GetComponent<EnemyThrow>().enabled = true;
                }
            }
        }
    }
    public static float CalculateDistance(float strenght)
    {
        return strenght * 200f;
    }
}

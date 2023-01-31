using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public List <GameObject> characters = new List<GameObject>(); 
    
    public static float CalculateDistance(float strenght)
    {
        return strenght * 200f;
    }
    private void SpawnPlayers() //every script on the characters must be disabled
    {
        if(FirebaseTest.instance.mySpot == "Player1") // if im player1 set up from player1 perspective
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
}

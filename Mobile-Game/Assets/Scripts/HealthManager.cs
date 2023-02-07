using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    // Start is called before the first frame update
    float health1 = 1f;
    float health2 = 1f;

    float damage = .25f;
    private void OnEnable()
    {
        FireBomb.PlayerHit += DealDamage;
    }
    private void OnDisable()
    {
        
    }
    private void DealDamage(string player)
    {
        if(player == "Player1")
        {
            health1 -= damage;
            //deal damage
        }
        else
        {
            health2 -= damage;
            //deal Damage
        }
        Debug.Log($"Player 1 health; {health1} || Player2 health; {health2}");

        if (health1 <= 0)
        {
            Debug.Log("Player2 wins");
        }
        else if( health2 <= 0)
        {
            Debug.Log("Player1 wins");
        }
    }
}

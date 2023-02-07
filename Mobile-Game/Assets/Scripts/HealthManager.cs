using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public List <GameObject> slider = new List<GameObject>();

    float health1 = 1f;
    float health2 = 1f;

    float damage = .25f;
    private void OnEnable()
    {
        FireBomb.PlayerHit += DealDamage;
    }
    private void OnDisable()
    {
        FireBomb.PlayerHit -= DealDamage;
    }
    private void DealDamage(string player)
    {
        if(player == "Player1")
        {
            health1 -= damage;
            slider[0].GetComponent<Slider>().value -= damage;
            //deal damage
        }
        else
        {
            health2 -= damage;
            slider[1].GetComponent<Slider>().value -= damage;
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

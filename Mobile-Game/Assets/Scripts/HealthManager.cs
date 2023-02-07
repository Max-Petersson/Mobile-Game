using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public List <GameObject> slider = new List<GameObject>();
    public GameObject WinScreen;
    public List<Sprite> characterIcons = new List<Sprite>();

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

        if (health1 <= 0)
        {
            ThrowProjectile.gameOver = true;
            FirebaseTest.instance.StopListeningForThrows();
            StartCoroutine(BackToMain());
            //show winscreen
            ShowWinScreen(1);
        }
        else if( health2 <= 0)
        {
            ThrowProjectile.gameOver = true;
            FirebaseTest.instance.StopListeningForThrows();
            //Show winscreen
            ShowWinScreen(0);
            StartCoroutine(BackToMain());
        }
    }
    IEnumerator BackToMain()
    {
        yield return new WaitForSeconds(3f);
        if(FirebaseTest.mySpot == "Player1")
        {
            FirebaseTest.instance.DestroyGameSession();
        }
        FirebaseTest.mySpot = "";
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    private void ShowWinScreen(int whichCharacter)
    {
        WinScreen.GetComponent<Image>().enabled = true;
        WinScreen.GetComponent<Image>().sprite = characterIcons[whichCharacter];
    }
}

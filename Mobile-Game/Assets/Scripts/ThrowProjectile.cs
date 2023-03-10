using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    public static event Action <float> ThrowFireBall;
    public static event Action <Vector3> RotateAim;
    public static event Action SpawnProjectile;
    public static event Action <ProjectileInfo>SendToFireBase;

    Vector3 anchorPoint = Vector3.zero;
    Vector3 endTouch = Vector3.zero;
    float strenght = 0;
    public static float rotz = 0f;
    public bool myTurn = true;
    public static bool gameOver = false;

    private void Start() // Hej mich
    {
        Aim.referencePoint = transform.position;
        gameOver = false;
    }
    private void OnEnable()
    {
        FireBomb.Explode += SwitchTurn;
    }
    private void OnDisable()
    {
        FireBomb.Explode -= SwitchTurn;
    }
    void Update()
    {
        if (myTurn == false && gameOver == false) return;
        if (Input.touchCount <= 0) return;


        Touch myTouch = Input.GetTouch(0);
        if (myTouch.phase == TouchPhase.Began)
        {
            anchorPoint = Camera.main.ScreenToWorldPoint(myTouch.position);
            SpawnProjectile?.Invoke();
        }

        RotateAim?.Invoke(Camera.main.ScreenToWorldPoint((Vector3)myTouch.position) - gameObject.transform.position);

        if (myTouch.phase == TouchPhase.Ended)
        {
            endTouch = Camera.main.ScreenToWorldPoint(myTouch.position);
            strenght = Vector3.SqrMagnitude(endTouch - anchorPoint);
            strenght = Mathf.Clamp(strenght, .1f, 10f);

            ThrowFireBall?.Invoke(strenght);
            myTurn = false; // so that you cant send another one.
            ProjectileInfo toFirebase = new ProjectileInfo(rotz, strenght);
            SendToFireBase?.Invoke(toFirebase);
        }
    }
   
    private void SwitchTurn()
    {
        if (myTurn == false)
        {
            myTurn = true;
        }
        else
            myTurn = false;
    }
    public void DebugTurn()
    {
        IEnumerator Switch()
        {
            yield return new WaitForSeconds(2f);
            if (myTurn == false)
            {
                myTurn = true;
            }
            else
                myTurn = false;
        }
        StartCoroutine(Switch());
    }
}

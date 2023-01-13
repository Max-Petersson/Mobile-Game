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

    Vector3 anchorPoint = Vector3.zero;
    Vector3 endTouch = Vector3.zero;
    float strenght = 0;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anchorPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            SpawnProjectile?.Invoke();
        }
        if (Input.GetMouseButton(0))
        {
            // visuals the bar of the throw mmeter going red
            RotateAim?.Invoke(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        if (Input.GetMouseButtonUp(0))
        {
            endTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            strenght = Vector3.SqrMagnitude(endTouch - anchorPoint);
            strenght = Mathf.Clamp(strenght, .1f, 10f);

            ThrowFireBall?.Invoke(strenght);
        }
    }
}

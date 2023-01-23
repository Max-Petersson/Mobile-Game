using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThrow : MonoBehaviour
{
    public GameObject prefab = null;
    bool shouldThrow = false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        FirebaseTest.OnProjectile += EnemyProjectile;
    }
    private void OnDisable()
    {
        FirebaseTest.OnProjectile -= EnemyProjectile;
    }
    public void EnemyProjectile(ProjectileInfo info)
    {
        if(shouldThrow == true)
        {
            transform.rotation = Quaternion.Euler(0, 0, info.angle);

            GameObject child = null;
            Vector2 pos;
            foreach (Transform t in transform)
            {
                child = t.gameObject;
            }
            pos = child.transform.position;
            GameObject fireball = Instantiate(prefab, pos, Quaternion.identity);

            float distanceMultiplyer = GameHandler.CalculateDistance(info.speed);
            fireball.GetComponent<Rigidbody2D>().AddForce(child.transform.right * -1 * distanceMultiplyer);
            shouldThrow = false;
        }
        else
            shouldThrow = true;

    }
}

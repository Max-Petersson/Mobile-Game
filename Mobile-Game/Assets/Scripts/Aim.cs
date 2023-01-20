using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    private void OnEnable()
    {
        ThrowProjectile.RotateAim += UpdateRotation;
        ThrowProjectile.SpawnProjectile += FireballSpawn;
        ThrowProjectile.ThrowFireBall += ShootProjectile;
    }
    private void OnDisable()
    {
        ThrowProjectile.RotateAim -= UpdateRotation;
        ThrowProjectile.SpawnProjectile -= FireballSpawn;
        ThrowProjectile.ThrowFireBall -= ShootProjectile;
    }
   
    private void UpdateRotation(Vector3 dir)
    {
        float rotz = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        ThrowProjectile.rotz = rotz;
        transform.rotation = Quaternion.Euler(0, 0, rotz);
    }
    private void FireballSpawn()
    {
        GameObject child = null;
        foreach(Transform t in transform)
        {
            child = t.gameObject;
        }

        child.gameObject.GetComponent<SpriteRenderer>().sprite = prefab.GetComponent<SpriteRenderer>().sprite;
    }
    private void ShootProjectile(float strength)
    {

        Vector2 pos = Vector2.zero;
        GameObject child = null;

        foreach (Transform t in transform)
        {
            child = t.gameObject;
        }
        pos = child.transform.position;

        GameObject projectile = null;
        projectile = Instantiate(prefab, pos, Quaternion.identity);

        float distanceMultiplyer = GameHandler.CalculateDistance(strength);
        projectile.GetComponent<Rigidbody2D>().AddForce(child.transform.right *-1 * distanceMultiplyer);
    }
   
}

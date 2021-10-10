using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField] private GameObject point;
    [SerializeField] private float speed;
    public void FireProjectile()
    {
        GameObject projectile = Instantiate(point, null);
        projectile.transform.position = this.transform.position;
        projectile.GetComponent<Rigidbody2D>().AddRelativeForce(-this.transform.right * speed);
    }
}

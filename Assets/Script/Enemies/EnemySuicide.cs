using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySuicide : EnemyShip
{
    [Space]
    [SerializeField] private float hitDamage = 1f;
    [SerializeField] private GameObject deathEffect;

    private new void Start()
    {
        base.Start();
        onDeath += SelfDestruct;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PirateShip>().TakeDamage(hitDamage);
            Instantiate(deathEffect, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    private void SelfDestruct()
    {
        Instantiate(deathEffect, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}

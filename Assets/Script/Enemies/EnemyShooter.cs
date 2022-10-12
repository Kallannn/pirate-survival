using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : EnemyShip
{
    [Space]
    [SerializeField] private EnumEnemyShooterMainAttack mainAttack = EnumEnemyShooterMainAttack.FrontCannon;
    [SerializeField] private GameObject deathEffect;

    private new void Start()
    {
        base.Start();
        whileStationary += ShootAtPlayer;

        onDeath += SelfDestruct;
    }

    private void ShootAtPlayer()
    {
        LookAtPlayer();
        switch (mainAttack)
        {
            case EnumEnemyShooterMainAttack.FrontCannon:
                ShootFrontalCannon();
                break;

            case EnumEnemyShooterMainAttack.SideCannon:
                thisRigidBody.rotation -= 90f;
                ShootSideCannons();
                break;
        }
    }

    private void SelfDestruct()
    {
        Instantiate(deathEffect, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

}

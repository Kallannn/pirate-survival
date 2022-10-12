using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float damage = 1f;
    private Rigidbody2D thisRigidBody;
    private Collider2D thisCollider;
    private Animator thisAnimator;

    private void Awake()
    {
        thisRigidBody = this.GetComponent<Rigidbody2D>();
        thisCollider = this.GetComponent<Collider2D>();
        thisAnimator = this.GetComponent<Animator>();
    }

    public void Go()
    {
        thisRigidBody.velocity = this.transform.up * this.speed;
        thisCollider.enabled = true;
    }

    public void Go(float damage)
    {
        this.damage = damage;
        Go();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        var ship = collision.gameObject.GetComponent<PirateShip>();

        if(ship != null)
        {
            ship.TakeDamage(damage);
        }

        Explode();

    }

    private void Explode()
    {
        thisRigidBody.velocity = Vector3.zero;
        thisCollider.enabled = false;
        thisAnimator.SetTrigger("Explosion");
    }

    private void EndOfExplosionAnimation_CalledByAnimationEvent()
    {
        
        this.gameObject.SetActive(false);
    }
}

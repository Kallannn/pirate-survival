using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PirateShip : MonoBehaviour
{
    [SerializeField] protected float life = 5f;
    [Space]
    [Header("Movement")]
    [SerializeField] protected EnumShipSpeedMode movementMode = EnumShipSpeedMode.Constant;
    [SerializeField] protected float acceleration = 1f;
    [SerializeField] protected float maxSpeed = 3f;
    [SerializeField] protected EnumShipRotationMode rotationMode = EnumShipRotationMode.TileBased;
    [SerializeField] protected float rotationSpeed = 100;
    [Space]
    [Header("Frontal Cannon Attack")]
    [SerializeField] protected PrefabPool frontalCannonBallPool;
    [SerializeField] protected float frontalCannonDamage = 1f;
    [SerializeField] protected float frontalCannonCooldown = 1f;
    [SerializeField] protected Transform frontalCannonFirepoint;
    [Space]
    [Header("Side Cannons Attack")]
    [SerializeField] protected PrefabPool sideCannonBallPool;
    [SerializeField] protected float sideCannonDamagePerShot = 1f;
    [SerializeField] protected float sideCannonCooldown = 2f;

    [Tooltip("This variable represents how many cannon balls the side cannons will shoot at each side at once. Default is 3.")]
    [SerializeField] protected int sideCannonShotsCount = 3;

    [Tooltip("This variable represents the angle difference between the two most imprecise shots to each side. Default is 30.")]
    [SerializeField] protected float sideCannonTotalSpread = 30f;

    private bool frontalCannonReady = true, sideCannonReady = true;
    protected Rigidbody2D thisRigidBody;

    private int[] tileBasedRotationAngles = new int[] { 315, 270, 225, 180, 135, 90, 45, 0 };
    private int currentTileRotation = 0;

    protected delegate void OnDeath();
    protected OnDeath onDeath;

    private Animator animator;

    private void Awake()
    {
        thisRigidBody = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        if(animator != null)
        {
            animator.SetFloat("Life", life);
        }

        this.currentTileRotation = 0;
        LockRotationIfTileBased();
    }

    #region rotation

    public void RotateShip(float direction)
    {
        switch (rotationMode)
        {
            case EnumShipRotationMode.Free:
                RotateFreely(direction);
                break;
            case EnumShipRotationMode.TileBased:
                RotateTileBasedly(direction);
                break;
            default:
                throw new System.Exception(this.gameObject.name + "'ship has no rotation mode selected.");
        }
    }

    private void RotateFreely(float direction)
    {
        this.transform.rotation *= Quaternion.Euler(new Vector3(0, 0, rotationSpeed * direction * -1 * Time.deltaTime));
    }

    private void RotateTileBasedly(float direction)
    {
        if (direction > 0)
        {
            currentTileRotation++;
            if (currentTileRotation > tileBasedRotationAngles.Length - 1) currentTileRotation = 0;

            this.transform.rotation = Quaternion.Euler(0, 0, tileBasedRotationAngles[currentTileRotation]);
        }
        else if (direction < 0)
        {
            currentTileRotation--;
            if (currentTileRotation < 0) currentTileRotation = tileBasedRotationAngles.Length - 1;

            this.transform.rotation = Quaternion.Euler(0, 0, tileBasedRotationAngles[currentTileRotation]);
        }
    }

    protected void LockRotationIfTileBased()
    {
        if (this.rotationMode == EnumShipRotationMode.TileBased)
        {
            this.thisRigidBody.freezeRotation = true;
        }
        else
        {
            this.thisRigidBody.freezeRotation = false;
        }
    }

    #endregion

    #region shooting

    public void ShootFrontalCannon()
    {
        if (this.frontalCannonReady)
        {
            Shoot(frontalCannonFirepoint.position, this.transform.rotation);

            this.frontalCannonReady = false;
            StartCoroutine(FrontalCannonCooldownTimer());
        }
    }

    public void ShootSideCannons()
    {
        if (this.sideCannonReady)
        {
            float spreadBetweenShots = sideCannonTotalSpread / (sideCannonShotsCount - 1);
            for (int i = 0; i < sideCannonShotsCount; i++)
            {
                if (sideCannonShotsCount % 2 == 0)
                {
                    Quaternion shotRotation = this.transform.rotation * Quaternion.Euler(0, 0, 90 + (spreadBetweenShots / 2) + ((i - sideCannonShotsCount / 2) * spreadBetweenShots));
                    Shoot(this.transform.position, shotRotation);

                    shotRotation *= Quaternion.Euler(0, 0, 180);
                    Shoot(this.transform.position, shotRotation);
                }
                else
                {
                    Quaternion shotRotation = this.transform.rotation * Quaternion.Euler(0, 0, 90 + ((i - (sideCannonShotsCount - 1) / 2) * spreadBetweenShots));
                    Shoot(this.transform.position, shotRotation);

                    shotRotation *= Quaternion.Euler(0, 0, 180);
                    Shoot(this.transform.position, shotRotation);
                }

            }

            this.sideCannonReady = false;
            StartCoroutine(SideCannonCooldownTimer());
        }
    }

    private void Shoot(Vector3 position, Quaternion rotation)
    {
        var cannBall = frontalCannonBallPool.GetPooledObject();
        cannBall.transform.position = position;
        cannBall.transform.rotation = rotation;
        cannBall.SetActive(true);
        cannBall.GetComponent<CannonBall>().Go();
    }

    #endregion

    #region movement

    protected void MoveShip(Vector3 inputsDirection)
    {
        switch (movementMode)
        {
            case EnumShipSpeedMode.Constant:
                MoveShip_ConstantSpeed(inputsDirection);
                break;
            case EnumShipSpeedMode.Variable:
                MoveShip_VariableSpeed(inputsDirection);
                break;
            default:
                throw new System.Exception(this.gameObject.name + "'ship has no movement mode selected.");

        }
    }

    private void MoveShip_ConstantSpeed(Vector3 inputsDirection)
    {
        thisRigidBody.velocity = this.transform.up * maxSpeed * inputsDirection.y;
    }

    private void MoveShip_VariableSpeed(Vector3 inputsDirection)
    {
        Vector2 forwardDirection = new Vector2(this.transform.up.x, this.transform.up.y);

        RedirectCurrentVelocityToForward(forwardDirection);

        thisRigidBody.AddForce(forwardDirection * acceleration * Time.deltaTime * inputsDirection.y, ForceMode2D.Impulse);

        ApplyMaxSpeedLimit();
    }

    private void RedirectCurrentVelocityToForward(Vector2 forwardDirection)
    {
        var currentVelocityMagnitude = thisRigidBody.velocity.magnitude;

        thisRigidBody.velocity = forwardDirection.normalized * currentVelocityMagnitude;

    }

    private void ApplyMaxSpeedLimit()
    {
        if (this.thisRigidBody.velocity.magnitude > maxSpeed)
        {
            this.thisRigidBody.velocity = this.thisRigidBody.velocity.normalized * maxSpeed;
        }
    }

    #endregion

    #region cooldownTimers

    private IEnumerator FrontalCannonCooldownTimer()
    {
        yield return new WaitForSeconds(frontalCannonCooldown);
        frontalCannonReady = true;
    }

    private IEnumerator SideCannonCooldownTimer()
    {
        yield return new WaitForSeconds(sideCannonCooldown);
        sideCannonReady = true;
    }

    #endregion

    public void TakeDamage(float damage)
    {
        this.life -= damage;

        if (animator != null) animator.SetFloat("Life", life);

        if (life <= 0)
        {
            if((life + damage > 0) && onDeath != null) onDeath();
        }
    }
}

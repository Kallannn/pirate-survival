using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : PirateShip
{
    [SerializeField] private Vector2 inputsDirection;
    [SerializeField] private GameManager gameManager;

    private bool alive => this.life > 0;
    private bool playerInputsEnabled = true;
    private void Start()
    {
        this.onDeath += Die;
    }
    
    private void Update()
    {
        if (alive && playerInputsEnabled)
        {
            GetPlayerInputs();
            RotateShip(inputsDirection.x);
            MoveShip(inputsDirection);
        }
    }

    private void GetPlayerInputs()
    {

        if (Input.GetKey(KeyCode.W)) inputsDirection.y = 1;
        else inputsDirection.y = 0;

        switch (this.rotationMode)
        {
            case EnumShipRotationMode.Free:
                if (Input.GetKey(KeyCode.D)) inputsDirection.x = 1;
                else if (Input.GetKey(KeyCode.A)) inputsDirection.x = -1;
                else inputsDirection.x = 0;
                break;

            case EnumShipRotationMode.TileBased:
                if (Input.GetKeyDown(KeyCode.D)) inputsDirection.x = 1;
                else if (Input.GetKeyDown(KeyCode.A)) inputsDirection.x = -1;
                else inputsDirection.x = 0;
                break;
        }
        


        if (Input.GetKey(KeyCode.Mouse0))
        {
            ShootFrontalCannon();
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            ShootSideCannons();
        }

    }

    private void Die()
    {
        thisRigidBody.velocity = Vector2.zero;

        gameManager.EndMatch("You Died");
    }

    public void SetGameManagerReference(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void DisableInputs()
    {
        playerInputsEnabled = false;
    }
}

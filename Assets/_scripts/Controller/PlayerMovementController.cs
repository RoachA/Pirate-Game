using System;
using System.Collections;
using System.Collections.Generic;
using Game.Systems;
using UnityEngine;

namespace Game.Player
{
    //https://www.youtube.com/watch?v=eL_zHQEju8s&t=529s ///makes it float on water
    //https://tomweiland.net/github
    //https://github.com/Habrador/Unity-Boat-physics-Tutorial

    /*Inertia: Ships don't stop or turn instantly—they have momentum.
    Gradual turning: Ships turn slowly rather than changing direction instantly.
    Acceleration and deceleration: Ships need time to speed up and slow down.
    Rotation based on input: Instead of just translating in the direction of input, the ship needs to rotate and move in its forward direction.
    Steps to Simulate Ship Movement:
    Use the direction vector for rotation control (i.e., the ship "points" towards a desired direction).
    Move the ship gradually forward based on its current facing direction (forward vector).
    Implement smooth turning, acceleration, and deceleration.*/
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private InputManager inputManager;
        [Space(10)]
        [SerializeField] private Rigidbody rigidBody;
        public float acceleration = 5f; // How fast the ship accelerates
        public float rotationAcceleration = 10f;
        public float turnSpeed = 50f; // How fast the ship turns
        public float maxSpeed = 10f; // Maximum speed of the ship
        public float deceleration = 2f; // How fast the ship slows down
        public float rotationDeceleration = 3f;
        private float currentSpeed = 0f; // Current speed of the ship
        private Vector2 inputDirection; // Direction vector from keyboard input
        private float currentTurnSpeed = 0f;

        [Header("Wind Debug")]
        [SerializeField] private Vector2 windDirection;
        [SerializeField] private float windPower = 1f;

        public struct WindData
        {
            public Vector2 Direction;
            public float Power;

            public WindData(Vector2 dir, float pow)
            {
                Direction = dir;
                Power = pow;
            }
        }

        public WindData GetWindData()
        {
            return new WindData(windDirection, windPower);
        }

        private void FixedUpdate()
        {
            inputDirection = inputManager.KeyboardInputDirectionVector;


            MoveShip();
            ApplyWind();
        }

        private void Update()
        {
            RotateShip();
        }

        private void RotateShip()
        {
            /*if (inputDirection != Vector2.zero)
            {
                rigidBody.AddTorque(new Vector3(0, inputDirection.x * turnSpeed, 0),
                    ForceMode.Acceleration);
            }
            else
            {
                rigidBody.angularVelocity =
                    Vector3.Lerp(rigidBody.angularVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            }*/

            float targetTurnSpeed = inputDirection.x * turnSpeed;
            currentTurnSpeed = Mathf.MoveTowards(currentTurnSpeed, targetTurnSpeed, rotationAcceleration * Time.deltaTime);

            if (inputDirection != Vector2.zero)
            {
                transform.Rotate(new Vector3(0, currentTurnSpeed * Time.deltaTime, 0));
            }
            else
            {
                currentTurnSpeed = Mathf.MoveTowards(currentTurnSpeed, 0f, rotationDeceleration * Time.deltaTime);
            }
        }

        private void MoveShip()
        {
            if (inputDirection.y != 0)
            {
                rigidBody.AddForce(transform.forward * (acceleration * inputDirection.y), ForceMode.Acceleration);
                rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);
            }
            else
            {
                rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            }
        }

        private void ApplyWind()
        {
            //broken
            rigidBody.velocity += new Vector3(windDirection.x, 0, windDirection.y) * windPower;
            // rigidBody.AddForce(new Vector3(windDirection.x, 0, windDirection.y) * windPower, ForceMode.Force);
        }
    }
}
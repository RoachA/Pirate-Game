using Game.Systems;
using UnityEngine;

namespace Game.Ship
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
    public class ShipMovementController : MonoBehaviour
    {
        [SerializeField] private bool isPlayer;

        [Header("Stats")]
        public float BaseAcceleration = 15f;
        public float BaseManeuverability = 15f;
        public float MaxSpeedBase = 10f; // Maximum speed of the ship

        [SerializeField] private InputManager inputManager;
        [Space(10)]
        [SerializeField] private Rigidbody rigidBody;
        public float acceleration = 5f; // How fast the ship accelerates
        public float rotationAcceleration = 10f;
        public float turnSpeed = 50f; // How fast the ship turns
        public float deceleration = 2f; // How fast the ship slows down
        public float rotationDeceleration = 3f;
        private Vector2 _inputDirection; // Direction vector from keyboard input
        public float _currentTurnSpeed = 0f;

        private Vector2 _maxRoll = new Vector2(-15, 15);


        [Header("Wind Debug")]
        [Range(0, 360)]
        [SerializeField] private float windAngle;
        [Range(0, 1)]
        [SerializeField] private float windPower = 1f;
        [SerializeField] private Material sailMat;

        public struct WindData
        {
            public float Direction;
            public float Power;

            public WindData(float dir, float pow)
            {
                Direction = dir;
                Power = pow;
            }
        }

        private void Start()
        {
            ApplyBaseStats();
        }

        private void ApplyBaseStats()
        {
            acceleration = BaseAcceleration;
        }

        public WindData GetWindData()
        {
            return new WindData(windAngle, windPower);
        }

        private void FixedUpdate()
        {
            _inputDirection = inputManager.KeyboardInputDirectionVector;

            //need to get these work for AI too.
            MoveShip();
            RotateShip();
            ApplyWind();
        }

        private void RotateShip()
        {
            float targetTurnSpeed = _inputDirection.x * turnSpeed;

            _currentTurnSpeed = Mathf.MoveTowards(_currentTurnSpeed, targetTurnSpeed, rotationAcceleration * Time.fixedDeltaTime);

            if (_inputDirection != Vector2.zero)
            {
                transform.Rotate(new Vector3(0, _currentTurnSpeed * Time.fixedDeltaTime, 0));
            }
            else
            {
                _currentTurnSpeed = Mathf.MoveTowards(_currentTurnSpeed, 0f, rotationDeceleration * Time.fixedDeltaTime);
            }

            //Roll
            // these values -20 and +20 may be affected by ship size etc later.
            float yawForceNormalized = Mathf.InverseLerp(-20, 20, _currentTurnSpeed);
            float roll = Mathf.Lerp(_maxRoll.x, _maxRoll.y, yawForceNormalized);
            float rbSpeedLimit = Mathf.InverseLerp(0, 30, rigidBody.velocity.magnitude);
            roll *= rbSpeedLimit;
            var currentEulerRotation = transform.eulerAngles;
            transform.eulerAngles = new Vector3(
                currentEulerRotation.x,
                currentEulerRotation.y,
                roll);
        }

        private void MoveShip()
        {
            if (_inputDirection.y != 0)
            {
                rigidBody.AddForce(transform.forward * (acceleration * _inputDirection.y), ForceMode.Acceleration);
                rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, MaxSpeedBase);
            }
            else
            {
                rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            }
        }

        private void ApplyWind()
        {
            float shipAngleY = transform.eulerAngles.y;
            float difference = Mathf.DeltaAngle(windAngle, shipAngleY);
            float absDifference = Mathf.Abs(difference);

            // Normalize the similarity between 0 and 1, where 0 means maximum difference and 1 means perfect alignment
            float similarity = Mathf.InverseLerp(180, 0, absDifference);


            sailMat.color = Color.Lerp(Color.red, Color.green, similarity);

            acceleration = (similarity * windPower) * BaseAcceleration;
            acceleration += 10f; //min acceleration when zero wind.

            rotationAcceleration = (similarity * windPower) * BaseManeuverability;
            rotationAcceleration += 8f; //min acceleration when zero wind.
        }
    }
}
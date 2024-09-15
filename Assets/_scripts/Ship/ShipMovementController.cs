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
    [RequireComponent(typeof(Ship))]
    public class ShipMovementController : MonoBehaviour
    {
        [SerializeField] private bool isPlayer;

        [Header("Stats")]
        public float BaseSpeed = 15f;
        public float BaseManeuverability = 15f;

        [SerializeField] private InputManager inputManager;
        [Space(10)]
        [SerializeField] private Rigidbody rigidBody;

        public float acceleration = 5f;
        public float torqueAcceleration = 10f;

        public float deceleration = 0.01f;
        public float rotationDeceleration = 0.01f;

        private float _currentTurnSpeed = 0f;
        private Vector2 _inputDirection; // Direction vector from keyboard input
        private Vector2 _maxRoll = new Vector2(-15, 15);
        private float MaxSpeedBase = 10f;
        private Ship _ship;
        private GameObject _shipBodyMesh;

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
            _ship = GetComponent<Ship>();
            ApplyBaseStats();
        }

        private void ApplyBaseStats()
        {
            var stats = _ship.ShipTypeData.Stats;

            BaseSpeed = stats.BaseSpeed;
            acceleration = BaseSpeed;
            BaseManeuverability = stats.BaseManeuverability;
            MaxSpeedBase = BaseSpeed * 3; // would change later.
            _shipBodyMesh = _ship.ShipBody;
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
            ApplySimpleBuoyancy();
        }

        private void RotateShip()
        {
            float targetTurnSpeed = _inputDirection.x * BaseManeuverability;

            _currentTurnSpeed = Mathf.MoveTowards(_currentTurnSpeed, targetTurnSpeed, torqueAcceleration * Time.fixedDeltaTime);

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

            acceleration = (similarity * windPower * 1.5f) * BaseSpeed;
            acceleration += 25f; //default wind effect

            torqueAcceleration = (similarity * windPower * 1.5f) * BaseManeuverability;
            torqueAcceleration += 15f; //default wind effect
        }

        [SerializeField] private float BuoyancyTime;
        [SerializeField] private float BuoyancyPosScale;
        [SerializeField] private float BuoyancyRotScale;

        private void ApplySimpleBuoyancy()
        {
            var time = Time.time * BuoyancyTime;
            float noiseValue = Mathf.PerlinNoise(time, 0f);

            float newY = (noiseValue - 0.5f) * BuoyancyPosScale;

            _shipBodyMesh.transform.localPosition = new Vector3(0, newY, 0);

            _shipBodyMesh.transform.localEulerAngles = new Vector3(0, 90, newY * BuoyancyRotScale);
        }
    }
}
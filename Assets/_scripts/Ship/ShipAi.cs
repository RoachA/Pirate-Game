using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Ship
{
    [RequireComponent(typeof(Ship))]
    [RequireComponent(typeof(ShipMovementController))]
    public class ShipAi : MonoBehaviour
    {
        public enum AIState
        {
            Patrol,
            Engage,
            Evade,
            Retreat
        }

        public AIState CurrentState = AIState.Patrol;
        public float ThreatLevel; // 0 - 100 
        public float CombatRange = 10;

        [SerializeField] private Ship TargetShip;
        [SerializeField] private ShipMovementController _movementController;
        public Vector2 InputDirectionVector { get; private set; }

        void Update()
        {
            switch (CurrentState)
            {
                case AIState.Patrol:
                    Patrol();
                    break;
                case AIState.Engage:
                    Engage();
                    break;
                case AIState.Evade:
                    Evade();
                    break;
                case AIState.Retreat:
                    Retreat();
                    break;
            }
        }

        private void Patrol()
        {
            _movementController = GetComponent<ShipMovementController>();
        }

        private void Engage()
        {
            //Position itself for an optimal attack.
            //Aim and shoot at the player.
            //Assess if the engagement is going well or if evasive maneuvers are needed.*/

            // Calculate optimal position relative to the player

            /*MoveToAttackPosition();
       
            // Fire at the player
            FireAtPlayer();
       
            // Check if the ship is taking heavy damage
            if (IsUnderHeavyAttack())
            {
                currentState = AIState.Evade;
            }*/
            var forwardPower = 1;
            var distanceToTarget = Vector3.Distance(TargetShip.transform.position, transform.position);
            if (distanceToTarget < CombatRange) forwardPower = 0;
            // var directionToTarget = (TargetShip.transform.position - transform.position).normalized;
            InputDirectionVector = new Vector2(0, forwardPower);
        }

        private void Evade()
        {
            //ManeuverToAvoidFire();

            /*if (ThreatLevelDecreases())
            {
                currentState = AIState.Engage;
            }*/
        }

        private void Retreat()
        {
            //MoveToRetreatPosition();

            /*if (IsSafe())
            {
                currentState = AIState.Patrol;
            }*/
        }

        private void OnDrawGizmos()
        {
            var thisPosition = transform.position;
            var targetPosition = TargetShip.transform.position;

            var distanceToTarget = Vector3.Distance(targetPosition, thisPosition);
            Gizmos.color = distanceToTarget < CombatRange ? Color.red : Color.green;
            Gizmos.DrawLine(thisPosition, TargetShip.ShipBody.transform.position);

            //forward
            Gizmos.color = Color.white;
            Gizmos.DrawLine(thisPosition, thisPosition + Vector3.forward * 40);
        }

        //check environment
        //asses enemy pos and obstacles
        //check self stats / durability / cannon count etc
        //check enemy stats / durability / cannon count etc

        //Check for Status Update:
        //make a decision -> analysis points yield a score that is used for ops.

        //A= engage
        //assess best shooting spot by checking range
        //try to keep distance
        //generate a route
        //provide navigate input for shipMovementController

        //B = retreat 
    }
}
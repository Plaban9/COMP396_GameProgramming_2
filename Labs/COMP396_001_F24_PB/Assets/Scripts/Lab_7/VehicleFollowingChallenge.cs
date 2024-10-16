using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Lab_7
{
    public class VehicleFollowingChallenge : MonoBehaviour
    {
        public Path path;
        public float speed = 10.0f;
        [Range(1.0f, 1000.0f)]
        public float steeringInertia = 100.0f;
        public bool isLooping = true;
        public float waypointRadius = 1.0f;
        //Actual speed of the vehicle
        private float curSpeed;
        [SerializeField] private int curPathIndex = 0;
        private int pathLength;
        private Vector3 targetPoint;
        Vector3 velocity;

        [SerializeField] private bool _isGoingForward = true;
        void Start()
        {
            pathLength = path.Length;
            velocity = transform.forward;
        }
        void Update()
        {
            //Unify the speed
            curSpeed = speed * Time.deltaTime;
            targetPoint = path.GetPoint(curPathIndex);
            //If reach the radius of the waypoint then move to next point in the path
            if (Vector3.Distance(transform.position, targetPoint) < waypointRadius)
            {
                UpdateCurrentPathIndex();
            }
            ////Move the vehicle until the end point is reached in the path
            //if (curPathIndex >= pathLength)
            //    return;
            //Calculate the next Velocity towards the path
            if (curPathIndex >= pathLength - 1 && !isLooping)
            {
                _isGoingForward = false;
                velocity += Steer(targetPoint, true);
            }
            else if (curPathIndex <= 0 && !isLooping)
            {
                _isGoingForward = true;
                velocity += Steer(targetPoint, true);
            }
            else
            {
                velocity += Steer(targetPoint);
            }
            //Move the vehicle according to the velocity
            transform.position += velocity;
            //Rotate the vehicle towards the desired Velocity
            transform.rotation = Quaternion.LookRotation(velocity);
        }
        public Vector3 Steer(Vector3 target, bool bFinalPoint = false)
        {
            //Calculate the directional vector from the current position towards the target point
            Vector3 desiredVelocity = (target - transform.position);
            float dist = desiredVelocity.magnitude;
            //Normalize the desired Velocity
            desiredVelocity.Normalize();
            //
            if (bFinalPoint && dist < waypointRadius)
            {
                //_isGoingForward = !_isGoingForward;
                desiredVelocity *= curSpeed * (dist / waypointRadius);
            }
            else
                desiredVelocity *= curSpeed;
            //Calculate the force Vector
            Vector3 steeringForce = desiredVelocity - velocity;
            return steeringForce / steeringInertia;
        }

        private void UpdateCurrentPathIndex()
        {
            curPathIndex = _isGoingForward || isLooping ? curPathIndex + 1 : curPathIndex - 1;

            //if (curPathIndex < pathLength - 1)
            //{
            //    curPathIndex++;
            //}
            //else 
            if (isLooping && curPathIndex > pathLength - 1)
            {
                curPathIndex = 0;
            }
            else
                return;
        }
    }
}

/*
 * Mirror Drone - manages mirror drone and his indicators
 * 
 * author: Marek Václavík
 * login: xvacla26
 * 
 */

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace UnityControllerForTello
{

    public class MirrorDrone : MonoBehaviour
    {
        const float NotAHitDistance = 100.0f;

        public float dangerFar = 1.0f, dangerMid = 0.6f, dangerClose = 0.3f;

        public Transform drone;
        public Transform speedIndicator;
        public Transform dangerIndicator;

        float speedVelocity ;
        Vector3 speedDirection;
        RaycastHit closestHit;
        private enum sites
        {
            left, frontleft, front, frontright, right, backright, back, backleft, top, bottom
        }
        sites hitDirection;

        // Start is called before the first frame update
        public void Start()
        {


        }

        /// <summary>
        /// Casts ray to direction a compares to clostst hit.
        /// </summary>
        /// <param name="SideVector">direction to cast ray.</param>
        void ManageRays(Vector3 sideVector, sites direction)
        {
            RaycastHit hit;
            Ray ray1 = new Ray(this.drone.transform.position, sideVector);
            if (Physics.SphereCast(ray1, 0.01f, out hit))
            {
                if (hit.transform.gameObject.layer == 31 ) //layer for spatialAwareness
                {
                    if (hit.distance < this.closestHit.distance)
                    {
                        this.closestHit = hit;
                        hitDirection = direction;
                    }
                }
            }
            
            
        }

        /// <summary>
        /// Calls ManageRays to distinct directions.
        /// </summary>
        void CheckObstacles()
        {
            this.closestHit.distance = NotAHitDistance;
            // Left
            ManageRays(-transform.right, sites.left);

            // Right
            ManageRays(transform.right, sites.right);

            //Forward
            ManageRays(transform.forward, sites.front);

            //Back
            ManageRays(-transform.forward, sites.back);

            //Up
            ManageRays(transform.up, sites.top);

            //Down
            ManageRays(-transform.up, sites.bottom);

            // Right front
            ManageRays(transform.right + transform.forward, sites.frontright);

            // backleft
            ManageRays(-(transform.right + transform.forward), sites.backleft);

            // frontleft
            ManageRays(transform.forward - transform.right, sites.frontleft);

            // backright
            ManageRays(-transform.forward + transform.right, sites.backright);
        }

        /// <summary>
        /// Changes direction of danger based on vector.
        /// </summary>
        /// <param name="DangerDirection">direction od danger.</param>
        void ManageDangerDirection(sites DangerDirection)
        {
            switch (DangerDirection)
            {
                case sites.back:
                    dangerIndicator.rotation = Quaternion.Euler(0, 180, 0);
                    break;
                case sites.backleft:
                    dangerIndicator.rotation = Quaternion.Euler(0, -135, 0);
                    break;
                case sites.backright:
                    dangerIndicator.rotation = Quaternion.Euler(0, 135, 0);
                    break;
                case sites.front:
                    dangerIndicator.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case sites.frontleft:
                    dangerIndicator.rotation = Quaternion.Euler(0, -45, 0);
                    break;
                case sites.frontright:
                    dangerIndicator.rotation = Quaternion.Euler(0, 45, 0);
                    break;
                case sites.left:
                    dangerIndicator.rotation = Quaternion.Euler(0, -90, 0);
                    break;
                case sites.right:
                    dangerIndicator.rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case sites.top:
                    dangerIndicator.rotation = Quaternion.Euler(-90, 0, 0);
                    break;
                case sites.bottom:
                    dangerIndicator.rotation = Quaternion.Euler(90, 0, 0);
                    break;

            }
        }

            // Update is called once per frame
            void Update()
        {
            //mirroring rotation
            transform.rotation = drone.rotation;

            speedVelocity = DroneSimulator.speedVelocity;
            speedDirection = DroneSimulator.speedDirection;
            
            //obtacles
            CheckObstacles();
            float distanceToObstacle = closestHit.distance;
            if (distanceToObstacle < dangerFar)
            {
                dangerIndicator.gameObject.SetActive(true);
                dangerIndicator.GetChild(0).gameObject.SetActive(true);
                dangerIndicator.GetChild(1).gameObject.SetActive(false);
                dangerIndicator.GetChild(2).gameObject.SetActive(false);

                ManageDangerDirection(hitDirection);
                if (distanceToObstacle < dangerClose)
                {
                    dangerIndicator.GetChild(0).gameObject.SetActive(true);
                    dangerIndicator.GetChild(1).gameObject.SetActive(true);
                    dangerIndicator.GetChild(2).gameObject.SetActive(true);
                }
                else if (distanceToObstacle < dangerMid)
                {
                    dangerIndicator.GetChild(0).gameObject.SetActive(true);
                    dangerIndicator.GetChild(1).gameObject.SetActive(true);
                    dangerIndicator.GetChild(2).gameObject.SetActive(false);
                }
            }
            else
            {
                dangerIndicator.gameObject.SetActive(false);
            }

            //speed indicator
            if (speedVelocity < 0.2f)
            {
                speedIndicator.gameObject.SetActive(false);
            }
            else
            {
                speedIndicator.gameObject.SetActive(true);
                speedIndicator.rotation = Quaternion.LookRotation(speedDirection);
                speedIndicator.GetChild(0).localScale = new Vector3(speedVelocity * 2, speedVelocity * 2, speedIndicator.GetChild(0).localScale.z); //size of tranform based on speedVelocity

            }

        }
    }
}


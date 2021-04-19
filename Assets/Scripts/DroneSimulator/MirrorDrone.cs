/*
 * Mirror Drone - manages mirror drone and hid indicators
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

static class Constants
{
    public const float NotAHitDistance = 100.0f;
}


namespace UnityControllerForTello
{
    public class MirrorDrone : MonoBehaviour
    {
        public float dangerFar = 1.0f, dangerMid = 0.6f, dangerClose = 0.3f;
        Transform drone;
        Transform speedIndicator;
        Transform dangerIndicator;
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
            this.drone = GameObject.Find("UnityDrone").transform;
            speedIndicator = GameObject.Find("SpeedIndicator").transform;
            dangerIndicator = GameObject.Find("DangerIndicator").transform;
            

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
                if (hit.transform.gameObject.layer == 31 )
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
            this.closestHit.distance = Constants.NotAHitDistance;
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
            CheckObstacles();
            Debug.Log("closestObstacle: " + this.closestHit.distance);
            transform.rotation = drone.rotation;
            speedVelocity = DroneSimulator.speedVelocity;
            speedDirection = DroneSimulator.speedDirection;
            float distanceToObstacle = closestHit.distance;
            if (distanceToObstacle < dangerFar)
            {
                dangerIndicator.gameObject.SetActive(true);
                dangerIndicator.GetChild(0).gameObject.SetActive(true);
                dangerIndicator.GetChild(1).gameObject.SetActive(false);
                dangerIndicator.GetChild(2).gameObject.SetActive(false);
                //Vector3 dangerDirection = (this.closestHit.transform.position - drone.position);
                //Debug.Log("smer: " + hitDirection);
                //float mirrorAngle = Mathf.Atan2(mirrorDir.y, mirrorDir.x) * Mathf.Rad2Deg;
                //dangerIndicator.rotation = Quaternion.LookRotation(hitDirection); //TODO..
                ManageDangerDirection(hitDirection);
                if (distanceToObstacle < dangerClose)
                {
                    dangerIndicator.GetChild(0).gameObject.SetActive(true);
                    dangerIndicator.GetChild(1).gameObject.SetActive(true);
                    dangerIndicator.GetChild(2).gameObject.SetActive(true);
                    //cubeRenderer.material.SetColor("_Color", Color.red);
                }
                else if (distanceToObstacle < dangerMid)
                {
                    dangerIndicator.GetChild(0).gameObject.SetActive(true);
                    dangerIndicator.GetChild(1).gameObject.SetActive(true);
                    dangerIndicator.GetChild(2).gameObject.SetActive(false);
                    //cubeRenderer.material.SetColor("_Color", Color.yellow);
                }
            }
            else
            {
                //cubeRenderer.material.SetColor("_Color", Color.green);
                dangerIndicator.gameObject.SetActive(false);
            }


            if (speedVelocity < 0.2f)
            {
                speedIndicator.gameObject.SetActive(false);
            }
            else
            {
                speedIndicator.gameObject.SetActive(true);
                speedIndicator.rotation = Quaternion.LookRotation(speedDirection);
                speedIndicator.GetChild(0).localScale = new Vector3(speedVelocity * 2, speedVelocity * 2, speedIndicator.GetChild(0).localScale.z);

            }

        }
    }
}


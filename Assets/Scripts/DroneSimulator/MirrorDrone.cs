using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityControllerForTello
{
    public class MirrorDrone : MonoBehaviour
    {
        // Start is called before the first frame update
        Transform drone;
        Transform speedIndicator;
        Transform dangerIndicator;
        Transform obstacle;
        Vector3 lastPosition = Vector3.zero;
        float speedVelocity ;
        Vector3 speedDirection;


        public void Start()
        {
            this.drone = GameObject.Find("UnityDrone").transform;
            speedIndicator = GameObject.Find("SpeedIndicator").transform;
            dangerIndicator = GameObject.Find("DangerIndicator").transform;
            

        }

        // Update is called once per frame
        void Update()
        {
            obstacle = GameObject.Find("Obstacle").transform;
            transform.rotation = drone.rotation;
            speedVelocity = DroneSimulator.speedVelocity;
            speedDirection = DroneSimulator.speedDirection;
            float distanceToObstacle = Vector3.Distance(obstacle.position, drone.position);
            if (distanceToObstacle < 3.0f)
            {
                dangerIndicator.gameObject.active = true;
                dangerIndicator.GetChild(0).gameObject.active = true;
                dangerIndicator.GetChild(1).gameObject.active = false;
                dangerIndicator.GetChild(2).gameObject.active = false;
                Vector3 mirrorDir = (obstacle.position - drone.position);
                //Debug.Log("smer: " + mirrorDir);
                //float mirrorAngle = Mathf.Atan2(mirrorDir.y, mirrorDir.x) * Mathf.Rad2Deg;
                dangerIndicator.rotation = Quaternion.LookRotation(mirrorDir);
                if (distanceToObstacle < 1.0f)
                {
                    dangerIndicator.GetChild(0).gameObject.active = true;
                    dangerIndicator.GetChild(1).gameObject.active = true;
                    dangerIndicator.GetChild(2).gameObject.active = true;
                    //cubeRenderer.material.SetColor("_Color", Color.red);
                }
                else if (distanceToObstacle < 2.0f)
                {
                    dangerIndicator.GetChild(0).gameObject.active = true;
                    dangerIndicator.GetChild(1).gameObject.active = true;
                    dangerIndicator.GetChild(2).gameObject.active = false;
                    //cubeRenderer.material.SetColor("_Color", Color.yellow);
                }
            }
            else
            {
                //cubeRenderer.material.SetColor("_Color", Color.green);
                dangerIndicator.gameObject.active = false;
            }

            //speedVelocity = ((drone.transform.position - lastPosition).magnitude) * 50;
            //speedDirection = ((drone.transform.position - lastPosition) * 50);

            if (speedVelocity < 0.2f)
            {
                speedIndicator.gameObject.active = false;
            }
            else
            {
                speedIndicator.gameObject.active = true;
                speedIndicator.rotation = Quaternion.LookRotation(speedDirection);
                speedIndicator.GetChild(0).localScale = new Vector3(speedVelocity * 2, speedVelocity * 2, speedIndicator.GetChild(0).localScale.z);

            }
            //update se zavola prumerne 3x behem 1 FixedUpdate..
            Debug.Log("Mirror speedDir: " + speedDirection);
            Debug.Log("mirror speedVel: " + speedVelocity);

            lastPosition = drone.transform.position;
        }
    }
}


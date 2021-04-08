using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityControllerForTello
{
    public class DroneSimulator : MonoBehaviour
    {
        public MirrorDrone mirrorDrone { get; private set; }
        Rigidbody rigidBody;
        InputController inputController;
        public float inputDrag, drag;
        SceneManager sceneManager;
        Vector3 lastPosition = Vector3.zero;
        public static float speedVelocity;
        public static Vector3 speedDirection;

        Transform rotors ;
        Transform drone;
        Transform canvas ;
        //Transform mirrorDrone;
        //Transform speedIndicator;
        //Transform danger;
        //RectTransform HUD ;
        //RectTransform arrow ;
        //Transform obstacle;
        //Transform MainCamera ;
        //Transform batteryIndicator;
        //Transform altitudeIndicator;

        // Use this for initialization
        public void CustomStart(SceneManager sceneManager)
        {
            this.sceneManager = sceneManager;
            rigidBody = transform.GetChild(0).GetComponent<Rigidbody>();
            inputController = sceneManager.inputController;
            //MainCamera = Camera.main.gameObject.transform;

            rotors = transform.GetChild(0).GetChild(3);
            drone = GameObject.Find("UnityDrone").transform;
            if (!drone)
                Debug.Log("Drone not found");

            //canvas = transform.GetChild(1);

            //HUD = transform.GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>();
            //arrow = transform.GetChild(1).GetChild(1).gameObject.GetComponent<RectTransform>();

            //mirrorDrone = GameObject.Find("UnityDrone").transform;
            //speedIndicator = mirrorDrone.GetChild(2);
            //danger = canvas.GetChild(0).GetChild(4).GetChild(3);

            //batteryIndicator = transform.GetChild(1).GetChild(0).GetChild(5).GetChild(0).GetChild(0);
            //altitudeIndicator = transform.GetChild(1).GetChild(0).GetChild(6).GetChild(0).GetChild(0);

            //obstacle = transform.GetChild(2);
            //mirrorDrone.CustomStart(drone);
        }

        // Update is called once per frame
        public void CustomUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                // Debug.Log("Button 1");
                ResetSimulator();
            }
        }

                //TODO
        public void FixedUpdate()
        {
                //toceni rotoru
                foreach (Transform rotor in rotors)
                {
                    rotor.transform.Rotate(0, 10.0f, 0);
                }
                //moyna vzuyivat received input
                rigidBody.AddForce(transform.up * 9.81f);
                bool receivingInput = false;
                var pitchInput = sceneManager.pitch;
                rigidBody.AddForce(drone.transform.forward * pitchInput);
                if (System.Math.Abs(pitchInput) > 0)
                {
                    receivingInput = true;
                }
                var elvInput = sceneManager.elv;
                rigidBody.AddForce(drone.transform.up * elvInput);
                if (System.Math.Abs(elvInput) > 0)
                {
                    Debug.Log("Receiving input TRUE elvInput > 0, elvinput: " + elvInput);
                    //transform.position += new Vector3(0, .1f, 0);
                    receivingInput = true;
                }
                var rollInput = sceneManager.roll;
                rigidBody.AddForce(drone.transform.right * rollInput);
                if (System.Math.Abs(rollInput) > 0)
                {

                    receivingInput = true;
                }

                var yawInput = sceneManager.yaw;
                rigidBody.AddTorque(drone.transform.up * yawInput);
                if (System.Math.Abs(yawInput) > 0)
                {

                    receivingInput = true;
                }

                if (receivingInput & rigidBody.drag != inputDrag)
                {
                    Debug.Log("receining input ,rigidBody.drag != inputDrag, inputDrag: " + inputDrag);
                    rigidBody.drag = inputDrag;
                    rigidBody.angularDrag = inputDrag ;
                }
                else if (!receivingInput & rigidBody.drag != drag)
                {
                    Debug.Log("NOT receining input ,rigidBody.drag != drag, drag: " + drag);
                    rigidBody.drag = drag;
                    rigidBody.angularDrag = drag * .9f;
                }
                drone.transform.rotation = Quaternion.Euler(10.0f * pitchInput, drone.transform.eulerAngles.y, -10.0f * rollInput);

            //mirrorDrone.transform.rotation = drone.transform.rotation;
            //Text distance = HUD.GetChild(0).gameObject.GetComponent<Text>();
            //float dist = Vector3.Distance(MainCamera.position, drone.position);
            //distance.text = Mathf.Round(dist * 100.0f) * 0.01f + "m";
            //    if (dist < 2.0f)
            //{

            //        //HUD.gameObject.GetComponent<Image>().enabled = false;
            //        mirrorDrone.gameObject.active = false;
            //}
            //    else
            //    {
            //    //HUD.gameObject.GetComponent<Image>().enabled = true;
            //    mirrorDrone.gameObject.active = true;


            //}
            //Text Altitude = HUD.GetChild(1).gameObject.GetComponent<Text>();
            //    float altit = Vector3.Distance(new Vector3(0.0f,transform.position.y,0.0f),new Vector3(0.0f, drone.position.y,0.0f));
            //    Altitude.text = Mathf.Round(altit * 100.0f) * 0.01f + "m";
            //    altitudeIndicator.GetChild(1).localScale = new Vector3(altitudeIndicator.GetChild(1).localScale.x, altit * 50, altitudeIndicator.GetChild(1).localScale.z);
            //float altitposition = -174.0f + (altit * 50)/2;
            //altitudeIndicator.GetChild(1).localPosition= new Vector3(altitudeIndicator.GetChild(1).localPosition.x, altitposition, altitudeIndicator.GetChild(1).localPosition.z);
            //HUD.GetChild(1).localPosition = new Vector3(HUD.GetChild(1).localPosition.x, -88 + (altit * 50) / 2, HUD.GetChild(1).localPosition.z);
            //var altitutudeRenderer = altitudeIndicator.GetChild(1).gameObject.GetComponent<Renderer>();
            //Debug.Log("vyska: " + altit);
            ////altitutudeRenderer.material.SetColor("_Color", Color.magenta);
            //if (altit < 0.5f)
            //{
            //    altitutudeRenderer.material.SetColor("_Color", Color.red);
            //}
            //else if (altit < 1.0f)
            //{
            //    altitutudeRenderer.material.SetColor("_Color", Color.yellow);
            //}
            //else
            //{
            //    altitutudeRenderer.material.SetColor("_Color", Color.green);
            //}
            ////Debug.Log("vyska, position: " + altitposition);
            //Text Speed = HUD.GetChild(2).gameObject.GetComponent<Text>();
            speedVelocity = ((drone.transform.position - lastPosition).magnitude)*50;
            speedDirection = ((drone.transform.position - lastPosition)*50);
            Debug.Log("speedDir: " + speedDirection);
            Debug.Log("speedVel: " + speedVelocity);
            ////speedIndicator.rotation = Quaternion.LookRotation(speedDirection);
            ////if (sp < 0.2f)
            ////{
            ////    speedIndicator.gameObject.active = false;
            ////}
            ////else
            ////{
            ////    speedIndicator.gameObject.active = true;
            ////    speedIndicator.rotation = Quaternion.LookRotation(speedDirection);
            ////}
            ////Debug.Log("speeddirect: " + speedDirection);

            //Speed.text = "Rychlost: " + Mathf.Round(sp * 100.0f) * 0.01f + "m/s";
            lastPosition = drone.transform.position;
            //    Text battery = HUD.GetChild(3).gameObject.GetComponent<Text>();
            //    float batt = (600 - Time.time)/6; //random
            //    battery.text = Mathf.Round(batt * 10.0f) * 0.1f + "%";
            //    if (batt < 15.0f)
            //    {
            //        batteryIndicator.GetChild(0).gameObject.active = false;
            //    }
            //    else if (batt < 35.0f)
            //    {
            //        batteryIndicator.GetChild(1).gameObject.active = false;
            //    }
            //else if (batt < 55.0f)
            //{
            //    batteryIndicator.GetChild(2).gameObject.active = false;
            //}
            //else if (batt < 75.0f)
            //{
            //    batteryIndicator.GetChild(3).gameObject.active = false;
            //}
            //else if (batt < 95.0f)
            //{
            //    batteryIndicator.GetChild(4).gameObject.active = false;
            //}
            //Vector3 viewPos = Camera.main.WorldToViewportPoint(drone.transform.position);
            //    viewPos.x = (viewPos.x - 0.5f)*2 + 0.5f;
            //    viewPos.y = (viewPos.y - 0.5f) * 2 + 0.5f;
            //    HUD.anchoredPosition = new Vector3(viewPos.x*1433, viewPos.y*920, 0.0f);
            //    bool isFullyVisible = IsVisibleFrom(HUD, Camera.main);
            //if (isFullyVisible)
            //    arrow.gameObject.active = false;
            //else
            //{
            //    arrow.gameObject.active = true;
            //    Vector3 targetDir = (HUD.anchoredPosition - new Vector2(716.0f, 206.0f)).normalized; 
            //    float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            //    arrow.localEulerAngles = new Vector3(0, 0, angle);
            //    arrow.anchoredPosition = new Vector3(716.0f, 206.0f, 0.0f) + (targetDir * 500.0f);
            //}
            //float distanceToObstacle = Vector3.Distance(obstacle.position, drone.position);
           // Debug.Log("vzdalenost: " + distanceToObstacle);
            //var cubeRenderer = danger.GetComponent<Renderer>();
            //danger
            //if (distanceToObstacle < 3.0f)
            //{
            //    danger.gameObject.active = true;
            //    danger.GetChild(0).gameObject.active = true;
            //    danger.GetChild(1).gameObject.active = false;
            //    danger.GetChild(2).gameObject.active = false;
            //    Vector3 mirrorDir = (obstacle.position - drone.position);
            //    //Debug.Log("smer: " + mirrorDir);
            //    //float mirrorAngle = Mathf.Atan2(mirrorDir.y, mirrorDir.x) * Mathf.Rad2Deg;
            //    danger.rotation = Quaternion.LookRotation(mirrorDir);
            //    if (distanceToObstacle < 1.0f)
            //    {
            //        danger.GetChild(0).gameObject.active = true;
            //        danger.GetChild(1).gameObject.active = true;
            //        danger.GetChild(2).gameObject.active = true;
            //        //cubeRenderer.material.SetColor("_Color", Color.red);
            //    }
            //    else if (distanceToObstacle < 2.0f)
            //    {
            //        danger.GetChild(0).gameObject.active = true;
            //        danger.GetChild(1).gameObject.active = true;
            //        danger.GetChild(2).gameObject.active = false;
            //        //cubeRenderer.material.SetColor("_Color", Color.yellow);
            //    }
            //}
            //else
            //{
            //    //cubeRenderer.material.SetColor("_Color", Color.green);
            //    danger.gameObject.active = false;
            //}

            //var cubeRenderer = obstacle.GetComponent<Renderer>();
            //cubeRenderer.material.SetColor("_Color", Color.red);


            //HUD.anchoredPosition = new Vector3(0.0f, 0.0f, 0.0f);


            //float yVel = rigidBody.velocity.y + Physics.gravity.y;

            //Howering
            // rigidBody.AddForce(0, -yVel * Time.deltaTime, 0, ForceMode.Acceleration);
        }

        public void ResetSimulator()
        {
            //transform.position = sceneManager.telloManager.transform.position; //TODO 0,0,0
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;

        }
    }

}
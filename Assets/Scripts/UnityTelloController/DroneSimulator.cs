using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityControllerForTello
{
    public class DroneSimulator : MonoBehaviour
    {
        Rigidbody rigidBody;
        InputController inputController;
        public float inputDrag, drag;
        SceneManager sceneManager;
        Vector3 lastPosition = Vector3.zero;

        Transform rotors ;
        Transform drone ;
        Transform canvas ;
        Transform mirrorDrone;
        Transform danger;
        RectTransform HUD ;
        RectTransform arrow ;
        Transform obstacle;
        Transform MainCamera ;
        // Use this for initialization
        public void CustomStart(SceneManager sceneManager)
        {
            this.sceneManager = sceneManager;
            rigidBody = transform.GetChild(0).GetComponent<Rigidbody>();
            inputController = sceneManager.inputController;
            rotors = transform.GetChild(0).GetChild(3);
            drone = transform.GetChild(0);
            canvas = transform.GetChild(1);
            mirrorDrone = canvas.GetChild(0).GetChild(4);
            danger = canvas.GetChild(0).GetChild(5);
            HUD = transform.GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>();
            arrow = transform.GetChild(1).GetChild(1).gameObject.GetComponent<RectTransform>();
            obstacle = transform.GetChild(2);
            MainCamera = Camera.main.gameObject.transform;
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

        /// <summary>
        /// Counts the bounding box corners of the given RectTransform that are visible from the given Camera in screen space.
        /// </summary>
        /// <returns>The amount of bounding box corners that are visible from the Camera.</returns>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="camera">Camera.</param>
        private int CountCornersVisibleFrom(RectTransform rectTransform, Camera camera)
        {
            Rect screenBounds = new Rect(0f, 0f, Screen.width, Screen.height); // Screen space bounds (assumes camera renders across the entire screen)
            Vector3[] objectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(objectCorners);

            int visibleCorners = 0;
            Vector3 tempScreenSpaceCorner; // Cached
            for (var i = 0; i < objectCorners.Length; i++) // For each corner in rectTransform
            {
                tempScreenSpaceCorner = camera.WorldToScreenPoint(objectCorners[i]); // Transform world space position of corner to screen space
                if (screenBounds.Contains(tempScreenSpaceCorner)) // If the corner is inside the screen
                {
                    visibleCorners++;
                }
            }
            return visibleCorners;
        }

        /// <summary>
        /// Determines if this RectTransform is fully visible from the specified camera.
        /// Works by checking if each bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
        /// </summary>
        /// <returns><c>true</c> if is fully visible from the specified camera; otherwise, <c>false</c>.</returns>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="camera">Camera.</param>
        public bool IsFullyVisibleFrom(RectTransform rectTransform, Camera camera)
        {
            return CountCornersVisibleFrom(rectTransform, camera) == 4; // True if all 4 corners are visible
        }

        /// <summary>
        /// Determines if this RectTransform is at least partially visible from the specified camera.
        /// Works by checking if any bounding box corner of this RectTransform is inside the cameras screen space view frustrum.
        /// </summary>
        /// <returns><c>true</c> if is at least partially visible from the specified camera; otherwise, <c>false</c>.</returns>
        /// <param name="rectTransform">Rect transform.</param>
        /// <param name="camera">Camera.</param>
        public bool IsVisibleFrom(RectTransform rectTransform, Camera camera)
        {
            return CountCornersVisibleFrom(rectTransform, camera) > 0; // True if any corners are visible
        }
        //TODO
        public void FixedUpdate()
        {
            //Transform rotors = transform.GetChild(0).GetChild(3);
            //Transform drone = transform.GetChild(0);
            //Transform canvas = transform.GetChild(1);
            //RectTransform HUD = transform.GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>();
            //RectTransform arrow = transform.GetChild(1).GetChild(1).gameObject.GetComponent<RectTransform>();
            //Transform obstacle = transform.GetChild(2);
            //Transform MainCamera = Camera.main.gameObject.transform;
                //if (rotors != null)
                //{
                //    Debug.Log("nasel jsem child ");
                //}
                //toceni rotoru
                foreach (Transform rotor in rotors)
                {
                    rotor.transform.Rotate(0, 10.0f, 0);
                }

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
                mirrorDrone.transform.rotation = drone.transform.rotation;
                Text distance = HUD.GetChild(0).gameObject.GetComponent<Text>();
                float dist = Vector3.Distance(MainCamera.position, drone.position);
                distance.text = "Vzdálenost: "+ Mathf.Round(dist * 100.0f) * 0.01f + "m";
                Text Altitude = HUD.GetChild(1).gameObject.GetComponent<Text>();
                float altit = Vector3.Distance(new Vector3(0.0f,transform.position.y,0.0f),new Vector3(0.0f, drone.position.y,0.0f));
                Altitude.text = "Výška: " + Mathf.Round(altit * 100.0f) * 0.01f + "m";
                Text Speed = HUD.GetChild(2).gameObject.GetComponent<Text>();
                float sp = ((drone.transform.position - lastPosition).magnitude)*50;
                Speed.text = "Rychlost: " + Mathf.Round(sp * 100.0f) * 0.01f + "m/s";
                lastPosition = drone.transform.position;
                Text battery = HUD.GetChild(3).gameObject.GetComponent<Text>();
                float batt = (600 - Time.time)/6;
                battery.text = "baterie: " + Mathf.Round(batt * 10.0f) * 0.1f + "%";
                Vector3 viewPos = Camera.main.WorldToViewportPoint(drone.transform.position);
                viewPos.x = (viewPos.x - 0.5f)*2 + 0.5f;
                viewPos.y = (viewPos.y - 0.5f) * 2 + 0.5f;
                HUD.anchoredPosition = new Vector3(viewPos.x*1433, viewPos.y*920, 0.0f);
                bool isFullyVisible = IsVisibleFrom(HUD, Camera.main);
            if (isFullyVisible)
                arrow.gameObject.active = false;
            else
            {
                arrow.gameObject.active = true;
                Vector3 targetDir = (HUD.anchoredPosition - new Vector2(716.0f, 206.0f)).normalized; 
                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                arrow.localEulerAngles = new Vector3(0, 0, angle);
                arrow.anchoredPosition = new Vector3(716.0f, 206.0f, 0.0f) + (targetDir * 500.0f);
            }
            float distanceToObstacle = Vector3.Distance(obstacle.position, drone.position);
            Debug.Log("vzdalenost: " + distanceToObstacle);
            //var cubeRenderer = danger.GetComponent<Renderer>();
            if (distanceToObstacle < 3.0f)
            {
                danger.gameObject.active = true;
                danger.GetChild(0).gameObject.active = true;
                danger.GetChild(1).gameObject.active = false;
                danger.GetChild(2).gameObject.active = false;
                Vector3 mirrorDir = (obstacle.position - drone.position);
                Debug.Log("smer: " + mirrorDir);
                //float mirrorAngle = Mathf.Atan2(mirrorDir.y, mirrorDir.x) * Mathf.Rad2Deg;
                danger.rotation = Quaternion.LookRotation(mirrorDir);
                if (distanceToObstacle < 1.0f)
                {
                    danger.GetChild(0).gameObject.active = true;
                    danger.GetChild(1).gameObject.active = true;
                    danger.GetChild(2).gameObject.active = true;
                    //cubeRenderer.material.SetColor("_Color", Color.red);
                }
                else if (distanceToObstacle < 2.0f)
                {
                    danger.GetChild(0).gameObject.active = true;
                    danger.GetChild(1).gameObject.active = true;
                    danger.GetChild(2).gameObject.active = false;
                    //cubeRenderer.material.SetColor("_Color", Color.yellow);
                }
            }
            else
            {
                //cubeRenderer.material.SetColor("_Color", Color.green);
                danger.gameObject.active = false;
            }

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
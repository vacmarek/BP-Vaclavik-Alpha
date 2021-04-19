/*
 * Scene Manager - manages the scene
 * 
 * author: Marek Václavík
 * login: xvacla26
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityControllerForTello
{
    public class SceneManager : MonoBehaviour
    {

        public DroneSimulator simulator { get; private set; }
        public HeadUpDisplay headUpDisplay { get; private set; }

        [HideInInspector]
        GameObject cameraFrame;
        GameObject canvas;
        GameObject missions;
        public Quaternion finalInputs { get; private set; }
        public float elv;
        public float yaw;
        public float pitch;
        public float roll;

        //TelloAutoPilot autoPilot; neni potreba
        public InputController inputController { get; private set; }


        void Awake()
        {
            //base.Awake();

            inputController = FindObjectOfType<InputController>();
            if (!inputController)
                Debug.LogError("Missing an input controller");
            else
                inputController.CustomAwake(this);

            //Simulator
            simulator = FindObjectOfType<DroneSimulator>();
            if (!simulator)
                Debug.Log("No simulator found");

            //HeadUpDisplay
            headUpDisplay = FindObjectOfType<HeadUpDisplay>();
            if (!headUpDisplay)
                Debug.Log("No HeadUpDisplay found");
            //CameraView
            cameraFrame = GameObject.Find("CameraFrame");
            if (!cameraFrame)
                Debug.Log("No CameraFrame found");
            //Canvas
            canvas = GameObject.Find("Canvas");
            if (!canvas)
                Debug.Log("No Canvas found");
            //Canvas
            missions = GameObject.Find("Missions");
            if (!missions)
                Debug.Log("No Missions found");
            Debug.Log("Begin Simul");            
        }

        private void Start()
        {
            inputController.CustomStart();
            Debug.Log("Begin Sim- START");
            simulator.CustomStart(this);
            headUpDisplay.CustomStart();
        }

        private void Update()
        {
            inputController.GetFlightCommmands();
            RunFrame();
          
        }

        public void Reset()
        {
            Debug.Log("Reset");
            simulator.ResetSimulator();

        }

        public void ToggleMesh()
        {
            Debug.Log("ToggleMesh");
            Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Spatial Awareness");

        }

        public void ChangeCamera()
        {
            Debug.Log("ChangeCamera");
            if (cameraFrame.transform.IsChildOf(headUpDisplay.transform))
            {
                cameraFrame.transform.SetParent(canvas.transform);
                cameraFrame.transform.localPosition = new Vector3(700,-150,0);
            }
            else
            {
                cameraFrame.transform.SetParent(headUpDisplay.transform);
                cameraFrame.transform.localPosition = new Vector3(500, 0, 0);
            }
                

        }

        public void ToggleMissions()
        {
            Debug.Log("ToggleMissions");
            if (missions.activeSelf)
            {
                missions.SetActive(false);
            }
            else
                missions.SetActive(true);
        }

        public void ToggleScale()
        {
            Debug.Log("ToggleScale");
            if (headUpDisplay.transform.localScale == new Vector3(1,1,1))
            {
                headUpDisplay.transform.localScale = new Vector3(1.5f, 1.5f, 1);
            }
            else
            {
                headUpDisplay.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        float timeSinceLastUpdate;
        float prevDeltaTime = 0;
        System.TimeSpan telloDeltaTime;
        public void RunFrame()
        {
            //Frame info
            //timeSinceLastUpdate = Time.time - prevDeltaTime;
            //prevDeltaTime = Time.time;
            //var deltaTime1 = (int)(timeSinceLastUpdate * 1000);
            //telloDeltaTime = new System.TimeSpan(0, 0, 0, 0, (deltaTime1));
            //inputs
            var inputs = inputController.CheckFlightInputs();


            finalInputs = CalulateFinalInputs(inputs.x, inputs.y, inputs.z, inputs.w);
            yaw = finalInputs.x;
            elv = finalInputs.y;
            roll = finalInputs.z;
            pitch = finalInputs.w;

           
        }

       
        Quaternion CalulateFinalInputs(float yaw, float elv, float roll, float pitch)
        {
            elv *= inputController.speed;
            roll *= inputController.speed;
            pitch *= inputController.speed;
            yaw *= inputController.speed;
            return new Quaternion(yaw, elv, roll, pitch);
        }

    }
}
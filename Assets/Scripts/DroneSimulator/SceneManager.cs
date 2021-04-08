﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityControllerForTello
{
    public class SceneManager : MonoBehaviour
    {

        public DroneSimulator simulator { get; private set; }
        public HeadUpDisplay headUpDisplay { get; private set; }

        [HideInInspector]

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
            //if in sim run the frame, else called from telloUpdate in flyonly
            RunFrame();
          
        }

        public void Reset()
        {
            Debug.Log("Reset");
            simulator.ResetSimulator();

        }

        float timeSinceLastUpdate;
        float prevDeltaTime = 0;
        System.TimeSpan telloDeltaTime;
        public void RunFrame()
        {
            //Frame info
            timeSinceLastUpdate = Time.time - prevDeltaTime;
            prevDeltaTime = Time.time;
            var deltaTime1 = (int)(timeSinceLastUpdate * 1000);
            telloDeltaTime = new System.TimeSpan(0, 0, 0, 0, (deltaTime1));
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
            //if (autoPilot.enabled)
            //    inputController.speed = 1;

            elv *= inputController.speed;
            roll *= inputController.speed;
            pitch *= inputController.speed;
            yaw *= inputController.speed;
            return new Quaternion(yaw, elv, roll, pitch);
        }

    }
}
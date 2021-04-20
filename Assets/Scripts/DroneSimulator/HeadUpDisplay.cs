/*
 * Head Up Display - manages visualizasion of HUD information
 * 
 * author: Marek Václavík
 * login: xvacla26
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UnityControllerForTello
{
    public class HeadUpDisplay : MonoBehaviour
    {
        Transform MainCamera;
        public Transform drone;
        public Transform altitudeIndicator;
        public Transform batteryIndicator;
        public Transform mirrorDrone;
        RectTransform HUD;
        public RectTransform navigationArrow;
        public RectTransform canvas;

        public Text distanceText;
        public Text altitudeText;
        public Text batteryText;


        float distanceToUser;
        float altitude;
        Renderer altitutudeRenderer;
        RectTransform altitudeTextTransform;

        // Start is called before the first frame update
        public void CustomStart()
        {
            MainCamera = Camera.main.gameObject.transform;
            HUD = transform.gameObject.GetComponent<RectTransform>();
            altitutudeRenderer = altitudeIndicator.gameObject.GetComponent<Renderer>();
            altitudeTextTransform = altitudeText.gameObject.GetComponent<RectTransform>();
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

        /// <summary>
        /// Via Raz() getting altitude of drone againt spatial mesh surface under drone.
        /// </summary>
        public void CheckAltitude()
        {
            RaycastHit hit;
            Ray ray1 = new Ray(this.drone.transform.position, Vector3.down);
            if (Physics.SphereCast(ray1, 0.01f, out hit))
            {
                if (hit.transform.gameObject.layer == 31)
                {
                    altitude = hit.distance;
                }
            }
        }

        /// <summary>
        /// Getting position of drone in screen and setting TARGET position.
        /// </summary>
        public void SetTarget()
        {
            Vector3 viewPos = Camera.main.WorldToScreenPoint(drone.transform.position);
            viewPos.x = (viewPos.x - (canvas.rect.width / 2)) * 2 + (canvas.rect.width / 2);
            viewPos.y = (viewPos.y - (canvas.rect.height / 2)) * 2 + (canvas.rect.height / 2);
            HUD.anchoredPosition = new Vector3(viewPos.x, viewPos.y, 0.0f);
        }

        // Update is called once per frame
        void Update()
        {
            SetTarget();

            distanceToUser = Vector3.Distance(MainCamera.position, drone.position);
            distanceText.text = Mathf.Round(distanceToUser * 100.0f) * 0.01f + "m";
            
            CheckAltitude();
            altitudeText.text = Mathf.Round(altitude * 100.0f) * 0.01f + "m";
            
            //size of altitude bar based on altitude value
            altitudeIndicator.localScale = new Vector3(altitudeIndicator.localScale.x, altitude * 50, altitudeIndicator.localScale.z);
            float altitposition = -174.0f + (altitude * 50) / 2;
            altitudeIndicator.localPosition = new Vector3(altitudeIndicator.localPosition.x, altitposition, altitudeIndicator.localPosition.z);

            //set altitudeText transform next to a bar
            altitudeTextTransform.localPosition = new Vector3(altitudeTextTransform.localPosition.x, -88 + (altitude * 50) / 2, altitudeTextTransform.localPosition.z); 
            
            //changing color of bar based on altitude
            
            if (altitude < 0.5f)
            {
                altitutudeRenderer.material.SetColor("_Color", Color.red);
            }
            else if (altitude < 1.0f)
            {
                altitutudeRenderer.material.SetColor("_Color", Color.yellow);
            }
            else
            {
                altitutudeRenderer.material.SetColor("_Color", Color.green);
            }

            //simulated battery capacity
            float batt = (600 - Time.time) / 6; 
            batteryText.text = Mathf.Round(batt * 10.0f) * 0.1f + "%";

            //baterry incicator bars
            if (batt < 15.0f)
            {
                batteryIndicator.GetChild(0).gameObject.SetActive(false);
            }
            else if (batt < 35.0f)
            {
                batteryIndicator.GetChild(1).gameObject.SetActive(false);
            }
            else if (batt < 55.0f)
            {
                batteryIndicator.GetChild(2).gameObject.SetActive(false);
            }
            else if (batt < 75.0f)
            {
                batteryIndicator.GetChild(3).gameObject.SetActive(false);
            }
            else if (batt < 95.0f)
            {
                batteryIndicator.GetChild(4).gameObject.SetActive(false);
            }
            

            //visibility of TARGET, showing navigation arrow
            bool isVisible = IsVisibleFrom(HUD, Camera.main);
            if (isVisible)
                navigationArrow.gameObject.SetActive(false);
            else
            {
                //((canvas.rect.width / 2), (canvas.rect.height / 2)) --) center of canvas
                navigationArrow.gameObject.SetActive(true);
                Vector3 targetDirection = (HUD.anchoredPosition - new Vector2((canvas.rect.width / 2), (canvas.rect.height / 2))).normalized;
                float angleTargetDirection = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                navigationArrow.localEulerAngles = new Vector3(0, 0, angleTargetDirection);
                navigationArrow.anchoredPosition = new Vector3((canvas.rect.width / 2), (canvas.rect.height / 2), 0.0f) + (targetDirection * 400.0f);
            }

            // based on distace to user, gameObject mirror drone is active or non-active
            if (distanceToUser < 1.0f)
            {

                mirrorDrone.gameObject.SetActive(false);
            }
            else
            {
                mirrorDrone.gameObject.SetActive(true);
            }


        }
    }

}

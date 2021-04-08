using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UnityControllerForTello
{
    public class HeadUpDisplay : MonoBehaviour
    {
        Transform MainCamera;
        Transform drone;
        Transform altitudeIndicator;
        Transform batteryIndicator;
        RectTransform HUD;
        RectTransform navigationArrow;
        Transform mirrorDrone;




        // Start is called before the first frame update
        public void CustomStart()
        {
            MainCamera = Camera.main.gameObject.transform;
            this.drone = GameObject.Find("UnityDrone").transform;
            altitudeIndicator = GameObject.Find("Altimeter").transform;
            batteryIndicator = GameObject.Find("BatteryIndicator").transform.GetChild(0).GetChild(0);
            navigationArrow = GameObject.Find("NavigationArrow").GetComponent<RectTransform>();
            HUD = transform.gameObject.GetComponent<RectTransform>();
            mirrorDrone = GameObject.Find("MirrorDrone").transform;

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

        // Update is called once per frame
        void Update()
        {
            float dist = Vector3.Distance(MainCamera.position, drone.position);
            Text distance = HUD.GetChild(0).gameObject.GetComponent<Text>(); // TODO
            distance.text = Mathf.Round(dist * 100.0f) * 0.01f + "m";
            //TODO
            Text Altitude = HUD.GetChild(1).gameObject.GetComponent<Text>();//TODO
            float altit = Vector3.Distance(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, drone.position.y, 0.0f));//todo pohrat si s podlahou
            Altitude.text = Mathf.Round(altit * 100.0f) * 0.01f + "m";

            altitudeIndicator.GetChild(0).GetChild(0).GetChild(1).localScale = new Vector3(altitudeIndicator.GetChild(0).GetChild(0).GetChild(1).localScale.x, altit * 50, altitudeIndicator.GetChild(0).GetChild(0).GetChild(1).localScale.z);
            float altitposition = -174.0f + (altit * 50) / 2;
            altitudeIndicator.GetChild(0).GetChild(0).GetChild(1).localPosition = new Vector3(altitudeIndicator.GetChild(0).GetChild(0).GetChild(1).localPosition.x, altitposition, altitudeIndicator.GetChild(0).GetChild(0).GetChild(1).localPosition.z);

            //posun textu vysky podle vysky
            HUD.GetChild(1).localPosition = new Vector3(HUD.GetChild(1).localPosition.x, -88 + (altit * 50) / 2, HUD.GetChild(1).localPosition.z);
            //vykreslovani barvy u vyskomeru podle vysky
            var altitutudeRenderer = altitudeIndicator.GetChild(0).GetChild(0).GetChild(1).gameObject.GetComponent<Renderer>();
            if (altit < 0.5f)
            {
                altitutudeRenderer.material.SetColor("_Color", Color.red);
            }
            else if (altit < 1.0f)
            {
                altitutudeRenderer.material.SetColor("_Color", Color.yellow);
            }
            else
            {
                altitutudeRenderer.material.SetColor("_Color", Color.green);
            }


            Text battery = HUD.GetChild(3).gameObject.GetComponent<Text>(); //TODO
            float batt = (600 - Time.time) / 6; //random
            battery.text = Mathf.Round(batt * 10.0f) * 0.1f + "%";
            if (batt < 15.0f)
            {
                batteryIndicator.GetChild(0).gameObject.active = false;
            }
            else if (batt < 35.0f)
            {
                batteryIndicator.GetChild(1).gameObject.active = false;
            }
            else if (batt < 55.0f)
            {
                batteryIndicator.GetChild(2).gameObject.active = false;
            }
            else if (batt < 75.0f)
            {
                batteryIndicator.GetChild(3).gameObject.active = false;
            }
            else if (batt < 95.0f)
            {
                batteryIndicator.GetChild(4).gameObject.active = false;
            }
            Vector3 viewPos = Camera.main.WorldToViewportPoint(drone.transform.position);
            viewPos.x = (viewPos.x - 0.5f) * 2 + 0.5f;
            viewPos.y = (viewPos.y - 0.5f) * 2 + 0.5f;
            HUD.anchoredPosition = new Vector3(viewPos.x * 1433, viewPos.y * 920, 0.0f);
            bool isFullyVisible = IsVisibleFrom(HUD, Camera.main);
            if (isFullyVisible)
                navigationArrow.gameObject.active = false;
            else
            {
                navigationArrow.gameObject.active = true;
                Vector3 targetDir = (HUD.anchoredPosition - new Vector2(716.0f, 206.0f)).normalized;
                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                navigationArrow.localEulerAngles = new Vector3(0, 0, angle);
                navigationArrow.anchoredPosition = new Vector3(716.0f, 206.0f, 0.0f) + (targetDir * 500.0f);
            }
            if (dist < 2.0f)
            {

                //HUD.gameObject.GetComponent<Image>().enabled = false;
                mirrorDrone.gameObject.active = false;
            }
            else
            {
                //HUD.gameObject.GetComponent<Image>().enabled = true;
                mirrorDrone.gameObject.active = true;
            }


            }
        }

}

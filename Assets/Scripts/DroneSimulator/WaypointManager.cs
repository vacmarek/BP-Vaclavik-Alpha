/*
 * Mirror Drone - manages rotation and value of distance to waypoints 
 * 
 * author: Marek Václavík
 * login: xvacla26
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaypointManager : MonoBehaviour {

    public Transform cameraToFace; 
    public Transform drone;
    TextMeshPro distance;

    void Start()
    {
        distance = transform.gameObject.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update() {
        transform.LookAt(transform.position + cameraToFace.transform.rotation * Vector3.forward, cameraToFace.transform.rotation * Vector3.up);
        float dist = Vector3.Distance(drone.position, transform.position);
        distance.text = Mathf.Round(dist * 100.0f) * 0.01f + "m";
    }
}

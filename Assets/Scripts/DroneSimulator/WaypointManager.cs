/*
 * Mirror Drone - manages mirror drone and hid indicators
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


    // Update is called once per frame
    void Update() {
        transform.LookAt(transform.position + cameraToFace.transform.rotation * Vector3.forward, cameraToFace.transform.rotation * Vector3.up);
        TextMeshPro distance = transform.gameObject.GetComponent<TextMeshPro>(); // TODO
        float dist = Vector3.Distance(drone.position, transform.position); ;
        distance.text = Mathf.Round(dist * 100.0f) * 0.01f + "m";
    }
}

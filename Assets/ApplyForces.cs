﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApplyForces : MonoBehaviour
{
    public List<GameObject> filesObjects;
    private float nextActionTime = 0.0f;
    private float interval = 2.0f;
    public float desiredConnectedDistance = 1.0f;
    private float connectedForce = 1.0F;
    private bool areFilesPlaced = false;

    // Start is called before the first frame update
    void Start()
    {
        filesObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {   
        print(connectedForce);
        if (!IsEmpty(filesObjects)) {
            ApplyFilesObjectsForces();
            foreach(GameObject go in filesObjects) {
                if (!areFilesPlaced) {
                    if (go.GetComponent<ReadText>() != null) {
                        Vector3 velocity = go.GetComponent<ReadText>().GetVelocity();
                        go.transform.localPosition += (velocity * Time.deltaTime)/5.0f;
                    } else {
                        Vector3 velocity = go.GetComponent<DisplayImage>().GetVelocity(); 
                        go.transform.localPosition += (velocity * Time.deltaTime)/5.0f;
                    }
                }
            }
            if ((Time.time - DropdownToVisual.GetTimeFilesSelected()) >= nextActionTime) {
                nextActionTime += interval;
                if (connectedForce > 0.1f) {
                    connectedForce -= 0.05f;
                } else {
                    areFilesPlaced = true;
                }
            }
        }
    }

    private void ApplyFilesObjectsForces() {
        foreach(GameObject go in filesObjects) {
            foreach (GameObject gobj in filesObjects) {
                if (go != gobj) {
                    Vector3 difference = go.transform.localPosition - gobj.transform.localPosition;
                    float distance = (float)Math.Sqrt(Math.Pow(difference.x, 2) + Math.Pow(difference.y, 2) + Math.Pow(difference.z, 2));
                    var appliedForce = connectedForce * Mathf.Log10(distance / desiredConnectedDistance);
                    if (gobj.GetComponent<ReadText>() != null) {
                        gobj.GetComponent<ReadText>().SetVelocity(appliedForce*Time.deltaTime*difference.normalized);
                    } else {
                        gobj.GetComponent<DisplayImage>().SetVelocity(appliedForce*Time.deltaTime*difference.normalized); 
                    } 
                }
            }
        }
    }

    private static bool IsEmpty<T>(List<T> list)
    {
        if (list == null) {
            return true;
        }
 
        return !list.Any();
    }

    public void AddObj(GameObject go) {
        filesObjects.Add(go);
    }

    public void RemoveObj(GameObject go) {
        filesObjects.Remove(go);
    }

    public void InitMovements() {
        areFilesPlaced = false;
        connectedForce = 1.0F;
        nextActionTime = 0.0f;
    }

}

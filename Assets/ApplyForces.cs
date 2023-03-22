using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApplyForces : MonoBehaviour
{
    private static List<GameObject> filesObjects;
    private float nextActionTime = 5.0f;
    private float finishedNextAction = 8.0f;
    private float interval = 2.0f;
    private float desiredConnectedDistance = 0.4f;
    private float desiredConnectedDistanceMax = 0.8f;
    private float distanceDesiredFromCenterMax = 0.5f;
    private float connectedForce = 1.0f;
    private bool areFilesPlaced = false;

    // Enum used to specified if there is an attraction or repulsive force applied to the concerned object
    private enum Mode { repulsion = -1, neutral = 0, attraction = 1 }

    // Start is called before the first frame update
    void Start()
    {
        filesObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {   

        if (!IsEmpty(filesObjects) || filesObjects.Count > 1) {
            if (!areFilesPlaced) {
                ApplyCenterForce();
                foreach(GameObject go in filesObjects) {
                    if (go.GetComponent<DisplayText>() != null) {
                        Vector3 velocity = go.GetComponent<DisplayText>().GetVelocity();
                        go.transform.localPosition += velocity / 10.0f;
                    } else {
                        Vector3 velocity = go.GetComponent<DisplayImage>().GetVelocity(); 
                        go.transform.localPosition += velocity / 10.0f;
                    }
                }
                CleanVelocities();
                ApplyFilesObjectsForces();
                foreach(GameObject go in filesObjects) {
                    if (go.GetComponent<DisplayText>() != null) {
                        Vector3 velocity = go.GetComponent<DisplayText>().GetVelocity();
                        go.transform.localPosition += velocity / 10.0f;
                    } else {
                        Vector3 velocity = go.GetComponent<DisplayImage>().GetVelocity(); 
                        go.transform.localPosition += velocity / 10.0f;
                    }
                }
                CleanVelocities();
                if ((Time.time - MainSceneHandler.GetTimeFilesSelected()) >= nextActionTime) {
                    nextActionTime += interval;
                    if (nextActionTime > finishedNextAction) {
                        areFilesPlaced = true;
                    }
                }
            } 
        } 
    }

    private void ApplyFilesObjectsForces() {
        foreach(GameObject go in filesObjects) {
            foreach (GameObject gobj in filesObjects) {
                if (go != gobj) {
                    Vector2 p1 = new Vector2(go.transform.localPosition.x, go.transform.localPosition.y);
                    Vector2 p2 = new Vector2(gobj.transform.localPosition.x, gobj.transform.localPosition.y);
                    Vector2 difference = p1 - p2;
                    float distance = difference.magnitude;
                    float mode = (float)Mode.neutral;
                    if (distance <= desiredConnectedDistance) {
                        mode = (float)Mode.repulsion;
                    }
                    if (distance >= desiredConnectedDistanceMax) {
                        mode = (float)Mode.attraction;
                    }
                    var appliedForce = difference.normalized * (connectedForce / distance) * mode;
                    if (gobj.GetComponent<DisplayText>() != null) {
                        gobj.GetComponent<DisplayText>().SetVelocity(appliedForce*Time.deltaTime);
                    } else {
                        gobj.GetComponent<DisplayImage>().SetVelocity(appliedForce*Time.deltaTime); 
                    }
                }
            }
        }
    }

    private void ApplyCenterForce() {
        Vector2 p1 = MainSceneHandler.GetGravityCenterPosition();
        float distanceDesiredFromCenter = 0.5f;
        foreach(GameObject go in filesObjects) {
            Vector2 p2 = new Vector2(go.transform.localPosition.x, go.transform.localPosition.y);
            Vector2 difference = p1 - p2;
            float distance = difference.magnitude;
            float mode = (float)Mode.neutral;
            if (distance >= distanceDesiredFromCenterMax) {
                mode = (float)Mode.attraction;
            }
            if (distance < distanceDesiredFromCenterMax) {
                mode = (float)Mode.repulsion;
            }
            var appliedForce = difference.normalized * (connectedForce / distance) * mode;
            if (go.GetComponent<DisplayText>() != null) {
                go.GetComponent<DisplayText>().SetVelocity(appliedForce*Time.deltaTime);
            } else {
                go.GetComponent<DisplayImage>().SetVelocity(appliedForce*Time.deltaTime); 
            }
        }
    }

    private void CleanVelocities() {
        foreach(GameObject go in filesObjects) {
            if (go.GetComponent<DisplayText>() != null) {
                go.GetComponent<DisplayText>().InitVelocity();
            } else {
                go.GetComponent<DisplayImage>().InitVelocity();
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

    public static void RemoveObj(GameObject go) {
        filesObjects.Remove(go);
    }

    public void RemoveAllObjects() {
        filesObjects.Clear();
    }

    public void InitMovements() {
        areFilesPlaced = false;
        connectedForce = 1.0f;
        nextActionTime = 0.0f;
    }

}

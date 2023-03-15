using System;
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
    private float connectedForce = 0.6f;
    private bool areFilesPlaced = false;

    // Start is called before the first frame update
    void Start()
    {
        filesObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (!IsEmpty(filesObjects) || filesObjects.Count > 1) {
            ApplyCenterForce();
            foreach(GameObject go in filesObjects) {
                if (!areFilesPlaced) {
                    if (go.GetComponent<ReadText>() != null) {
                        Vector3 velocity = go.GetComponent<ReadText>().GetVelocity();
                        go.transform.localPosition += velocity * Time.deltaTime;
                    } else {
                        Vector3 velocity = go.GetComponent<DisplayImage>().GetVelocity(); 
                        go.transform.localPosition += velocity * Time.deltaTime;
                    }
                }
            }
            ApplyFilesObjectsForces();
            foreach(GameObject go in filesObjects) {
                if (!areFilesPlaced) {
                    if (go.GetComponent<ReadText>() != null) {
                        Vector3 velocity = go.GetComponent<ReadText>().GetVelocity();
                        go.transform.localPosition += velocity * Time.deltaTime;
                    } else {
                        Vector3 velocity = go.GetComponent<DisplayImage>().GetVelocity(); 
                        go.transform.localPosition += velocity * Time.deltaTime;
                    }
                }
            }
            if ((Time.time - DropdownToVisual.GetTimeFilesSelected()) >= nextActionTime) {
                nextActionTime += interval;
                if (connectedForce > 0.1f) {
                    connectedForce -= 0.2f;
                } else {
                    areFilesPlaced = true;
                }
            }

            // Removing rigidbodies to enable bounds controls
            if (areFilesPlaced) {
                foreach(GameObject go in filesObjects) {
                    Destroy(go.GetComponent<Rigidbody>());
                }
            }
        } 
    }

    private void ApplyFilesObjectsForces() {
        foreach(GameObject go in filesObjects) {
            foreach (GameObject gobj in filesObjects) {
                if (go != gobj) {
                    Vector3 p1 = go.transform.localPosition;
                    Vector3 p2 = gobj.transform.localPosition;
                    Vector3 difference = p1 - p2;
                    float distance = difference.magnitude;
                    if (gobj.GetComponent<ReadText>() != null) {
                        if (gobj.GetComponent<ReadText>().IsCollided()) {
                            distance = 0.05f;
                        }
                        float appliedForce = connectedForce * Mathf.Log10(distance / desiredConnectedDistance);
                        gobj.GetComponent<ReadText>().SetVelocity(appliedForce/10.0f*Time.deltaTime*difference.normalized);
                    } else {
                        if (gobj.GetComponent<DisplayImage>().IsCollided()) {
                            distance = 0.05f;
                        }
                        var appliedForce = connectedForce * Mathf.Log10(distance / desiredConnectedDistance);
                        gobj.GetComponent<DisplayImage>().SetVelocity(appliedForce/10.0f*Time.deltaTime*difference.normalized); 
                    }
                }
            }
        }
    }

    private void ApplyCenterForce() {
        Vector3 p1 = DropdownToVisual.GetGravityCenterPosition();
        foreach(GameObject go in filesObjects) {
            Vector3 p2 = go.transform.localPosition;
            Vector3 difference = p1 - p2;
            float distance = difference.magnitude;
            if (go.GetComponent<ReadText>() != null) {
                var appliedForce = connectedForce * Mathf.Log10(distance / desiredConnectedDistance);
                go.GetComponent<ReadText>().SetVelocity(appliedForce*Time.deltaTime*difference.normalized);
            } else {
                var appliedForce = connectedForce * Mathf.Log10(distance / desiredConnectedDistance);
                go.GetComponent<DisplayImage>().SetVelocity(appliedForce*Time.deltaTime*difference.normalized); 
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

    public void RemoveAllObjects() {
        filesObjects.Clear();
    }

    public void InitMovements() {
        areFilesPlaced = false;
        connectedForce = 1.0F;
        nextActionTime = 0.0f;
    }

}

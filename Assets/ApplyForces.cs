using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApplyForces : MonoBehaviour
{
    public List<GameObject> filesObjects;
    private Vector3 velocity;
    public float desiredConnectedDistance = 1.0f;
    public float connectedForce = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        filesObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {   
        if (!IsEmpty(filesObjects)) {
            ApplyFilesObjectsForces();
            foreach(GameObject go in filesObjects) {
                if (go.GetComponent<ReadText>() != null) {
                    Vector3 velocity = go.GetComponent<ReadText>().GetVelocity();
                    go.transform.localPosition += velocity * Time.deltaTime;
                } else {
                    Vector3 velocity = go.GetComponent<DisplayImage>().GetVelocity(); 
                    go.transform.localPosition += velocity * Time.deltaTime;
                }
            }
        } 
    }

    private void ApplyFilesObjectsForces() {
        foreach(GameObject go in filesObjects) {
            foreach (GameObject gobj in filesObjects) {
                if (go != gobj) {
                    if (filesObjects.Count <= 4) {
                        Vector3 difference = go.transform.localPosition - gobj.transform.localPosition;
                        float distance = (float)Math.Sqrt(Math.Pow(difference.x, 2) + Math.Pow(difference.y, 2) + Math.Pow(difference.z, 2));
                        var appliedForce = connectedForce * Mathf.Log10(distance / desiredConnectedDistance);
                        if (gobj.GetComponent<ReadText>() != null) {
                            gobj.GetComponent<ReadText>().SetVelocity(appliedForce*Time.deltaTime*difference.normalized);
                        } else {
                            gobj.GetComponent<DisplayImage>().SetVelocity(appliedForce*Time.deltaTime*difference.normalized); 
                        }
                    } else {
                        Vector3 difference = go.transform.localPosition - gobj.transform.localPosition;
                        var appliedForce = connectedForce / 10.0f;
                        if (gobj.GetComponent<ReadText>() != null) {
                            gobj.GetComponent<ReadText>().SetVelocity(appliedForce*Time.deltaTime*difference);
                        } else {
                            gobj.GetComponent<DisplayImage>().SetVelocity(appliedForce*Time.deltaTime*difference); 
                        }
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

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApplyForces : MonoBehaviour
{
    // List of lists of prefabs containing images or texts for each zone
    private static List<List<GameObject>> filesObjects;
    // Time to do another action
    private float nextActionTime = 5.0f;
    // Time after which one files are placed
    private float finishedNextAction = 8.0f;
    // Interval of time added to the nextActionTime
    private float interval = 2.0f;
    // Minimal distance between two files
    private float desiredConnectedDistance = 12.0f;
    // Maximal distance between two files
    private float desiredConnectedDistanceMax = 17.0f;
    // Maximal distance between a file and the center of the zone concerned
    private float distanceDesiredFromCenterMax = 15.0f;
    // Constant of force applied to a file
    private float connectedForce = 1.0f;
    // Boolean to know if the system of forces must be stopped 
    private bool areFilesPlaced = false;

    // Enum used to specified if there is an attraction, repulsive or none force applied to the concerned object
    private enum Mode { repulsion = -1, neutral = 0, attraction = 1 }

    // Start is called before the first frame update
    void Start()
    {
        filesObjects = new List<List<GameObject>>();
        for (int i=0; i<Heatmap.GetNumberOfZones(); i++) 
        {
            filesObjects.Add(new List<GameObject>());
        }
    }

    // Update is called once per frame
    void Update()
    {   
        // Distances are augmented to get a long range to see a lot of files (after 12 opened)
        if (filesObjects[0].Count >= 12 || filesObjects[1].Count >= 10 || filesObjects[2].Count >= 10) 
        {
            distanceDesiredFromCenterMax = 21.0f;
            desiredConnectedDistance = 18.0f;
            desiredConnectedDistanceMax = 23.0f;
        }

        if (!IsEmpty(filesObjects[0]) || filesObjects[0].Count > 1) 
        {
            if (!areFilesPlaced) 
            {
                foreach(List<GameObject> list in filesObjects)
                {
                    // Forces only between objects and center
                    ApplyCenterForce();
                    foreach(GameObject go in list) 
                    {
                        if (go.GetComponent<DisplayText>() != null) 
                        {
                            Vector3 velocity = go.GetComponent<DisplayText>().GetVelocity();
                            go.transform.localPosition += velocity / 10.0f;
                        } else {
                            Vector3 velocity = go.GetComponent<DisplayImage>().GetVelocity(); 
                            go.transform.localPosition += velocity / 10.0f;
                        }
                    }
                    CleanVelocities();

                    // Forces only between objects
                    ApplyFilesObjectsForces();
                    foreach(GameObject go in list) 
                    {
                        if (go.GetComponent<DisplayText>() != null) 
                        {
                            Vector3 velocity = go.GetComponent<DisplayText>().GetVelocity();
                            go.transform.localPosition += velocity;
                        } else {
                            Vector3 velocity = go.GetComponent<DisplayImage>().GetVelocity(); 
                            go.transform.localPosition += velocity;
                        }
                    }
                    CleanVelocities();
                }
                
                if ((Time.time - MainSceneHandler.GetTimeFilesSelected()) >= nextActionTime) 
                {
                    nextActionTime += interval;
                    if (nextActionTime > finishedNextAction) 
                    {
                        areFilesPlaced = true;
                    }
                }
            } 
        } 
    }

    /**
    * Method to apply a force between a file object and another.
    **/
    private void ApplyFilesObjectsForces() 
    {
        foreach (List<GameObject> list in filesObjects)
        {
            foreach(GameObject go in list) 
            {
                foreach (GameObject gobj in list) 
                {
                    if (go != gobj) 
                    {
                        // Positions of each object along x and y axis
                        Vector2 p1 = new Vector2(go.transform.localPosition.x, go.transform.localPosition.y);
                        Vector2 p2 = new Vector2(gobj.transform.localPosition.x, gobj.transform.localPosition.y);
                        // Difference between these positions 
                        Vector2 difference = p1 - p2;
                        float distance = difference.magnitude;
                        float mode = (float)Mode.neutral;
                        if (distance <= desiredConnectedDistance) 
                        {
                            mode = (float)Mode.repulsion;
                        }
                        if (distance >= desiredConnectedDistanceMax) 
                        {
                            mode = (float)Mode.attraction;
                        }
                        // Difference.normalize to obtain the direction
                        var appliedForce = difference.normalized * (connectedForce / distance) * mode;
                        if (gobj.GetComponent<DisplayText>() != null) 
                        {
                            gobj.GetComponent<DisplayText>().SetVelocity(appliedForce);
                        } else {
                            gobj.GetComponent<DisplayImage>().SetVelocity(appliedForce); 
                        }
                    }
                }
            }
        }
    }

    /**
    * Method to apply a force between a file object and the center of the most looked zone
    **/
    private void ApplyCenterForce() 
    {
        for(int i=0; i<Heatmap.GetNumberOfZones(); i++) 
        {
            // Center is located locally in 0.0f 
            Vector2 p1 = new Vector2(0.0f, 0.0f);
            foreach(GameObject go in filesObjects[i]) 
            {
                Vector2 p2 = new Vector2(go.transform.localPosition.x, go.transform.localPosition.y);
                Vector2 difference = p1 - p2;
                float distance = difference.magnitude;
                float mode = (float)Mode.neutral;
                if (distance >= distanceDesiredFromCenterMax) 
                {
                    mode = (float)Mode.attraction;
                }
                if (distance < distanceDesiredFromCenterMax) 
                {
                    mode = (float)Mode.repulsion;
                }
                var appliedForce = difference.normalized * (connectedForce / distance) * mode;
                if (go.GetComponent<DisplayText>() != null) 
                {
                    go.GetComponent<DisplayText>().SetVelocity(appliedForce*Time.deltaTime);
                } else {
                    go.GetComponent<DisplayImage>().SetVelocity(appliedForce*Time.deltaTime); 
                }
            }
        } 
    }

    /**
    * Put all velocities for each object to 0.0f
    **/
    private void CleanVelocities() 
    {
        foreach(List<GameObject> list in filesObjects) 
        {
            foreach(GameObject go in list) 
            {
                if (go.GetComponent<DisplayText>() != null) 
                {
                    go.GetComponent<DisplayText>().InitVelocity();
                } else {
                    go.GetComponent<DisplayImage>().InitVelocity();
                }
            }
        }
    }

    /**
    * Method to know if a list is empty or not
    * @param list : list of objects to verify
    *
    * @return true if the list is empty
    **/ 
    private static bool IsEmpty<T>(List<T> list)
    {
        if (list == null) 
        {
            return true;
        }
 
        return !list.Any();
    }

    /** 
    * Add a game object file to the list of the specified zone
    * @param go : game object to add 
    **/
    public void AddObj(GameObject go, int indexZone) 
    {
        filesObjects[indexZone].Add(go);
    }

    /**
    * Remove a game object file to the list of the specified zone
    * @param go : game object to remove 
    **/
    public static void RemoveObj(GameObject go, int indexZone) 
    {
        filesObjects[indexZone].Remove(go);
    }

    /**
    * Remove all game objects from each list of game objects file
    **/
    public void RemoveAllObjects() 
    {
        foreach(List<GameObject> l in filesObjects) {
            l.Clear();
        }
    }

    /**
    * Method to init variables important after each click on "visualize" button
    **/ 
    public void InitMovements() 
    {
        areFilesPlaced = false;
        nextActionTime = 0.0f;
    }
}

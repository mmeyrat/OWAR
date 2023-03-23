// Alan Zucconi
// www.alanzucconi.com
// doc : https://forum.unity.com/threads/how-to-create-heatmap-in-unity.423163/
// https://www.alanzucconi.com/2016/01/27/arrays-shaders-heatmaps-in-unity3d/#more-2003

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;


public class Heatmap : MonoBehaviour
{
    private Vector3[] positions;
    private Vector2[] properties;
    private Vector3[] orientations;
    private GameObject[] zones;
    
    // Number of points in the heatmap (Max is 1024 if we use the shader)
    public int count = 100;

    private int cpt = 0;

    // The time to wait before to update the map
    private float nextActionTime = 0.0f;

    // Period of time after each one we add a point in the map 
    private float period = 0.1f;

    // Get menu to not count when user is looking the menu 
    public GameObject menu;
    public GameObject displayer;

    // Timer of 30 seconds to scan around (lower to test)
    private float timeRemaining = 10.0f;
    
    private bool canBeUpdated = false;
    void Start ()
    {
        positions = new Vector3[count];
        properties = new Vector2[count];
        orientations = new Vector3[count];
        zones = new GameObject[count];

        for (int i = 0; i < positions.Length; i++)
        {
            // pos = (x, y, z)
            // At the launch the pointer is in the center so we initialize all points to 0.0f
            positions[i] = new Vector3(0.0f, 0.0f, 0.0f);
            
            // x = radius 
            // y = intensity
            // radius is 0.05 to be enough large and no too large
            // intensity will correspond to time the user look at this particular point 
            properties[i] = new Vector2(0.05f, 0.0f);

            // Initialise orientations with 0.0f for each positions
            orientations[i] = new Vector3(0, 0, 0);

            // Adding spheres at each position looked to prevent user (all spheres are at 0.0f for the begining and invisible)
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localPosition = new Vector3(positions[i].x, positions[i].x, positions[i].x);
            sphere.transform.localScale = new Vector3(properties[i].x, properties[i].x, properties[i].x);

            // Init the color of these sphere to green (because user never look here)
            // They become more and more red if user look more at this position
            Color customColor = new Color(0.0f, 1.0f, 0.0f, 0.0f);
            sphere.GetComponent<Renderer>().material.SetColor("_Color", customColor);
            sphere.SetActive(false);
            zones[i] = sphere;
        }
    }
 
    /*
    * Instead of adding points precisely, we can define a perimeter and if the user is looking
    * in this perimeter we add intensity to the concerned point. 
    * Let be this perimeter like 0.5 
    */
    void Update()
    {
        if (cpt < count && timeRemaining > 0.0f) {
            menu.SetActive(false);
            timeRemaining -= Time.deltaTime;

            if (Time.time > nextActionTime ) { 
                nextActionTime = Time.time + period;

                // Update positions points where the user is looking
                Vector3 pointLooked = CoreServices.InputSystem.GazeProvider.HitPosition;
                
                // Direction where the user is looking 
                Vector3 lookDirection = -1.0f * CoreServices.InputSystem.EyeGazeProvider.HitNormal;

                // we put the point where the user is looking in to the map 
                bool isAlreadyLooked = Array.Exists(positions, point => 
                    {
                        if (distance(point, pointLooked) < 0.25) {
                            int index = Array.IndexOf(positions, point);
                            properties[index] += new Vector2(0.0f, 0.1f); 
                            if (zones[index].transform.localScale.x < 0.35) {
                                zones[index].transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
                            }
                            zones[index].GetComponent<Renderer>().material.color += new Color(0.015f, -0.015f, 0.0f, 0.0f);
                            return true;
                        } else {
                            return false;
                        }
                    }
                );

                float semiWidth = menu.transform.localScale.x / 2.0f;
                double diag = Math.Sqrt(Math.Pow(2 * semiWidth, 2));
                bool isLookingMenu = distance(menu.transform.position, pointLooked) < diag;
                if (!isAlreadyLooked && !isLookingMenu) {
                    positions[cpt] = pointLooked;

                    // Update the intensity of this point a little bit 
                    properties[cpt] += new Vector2(0.0f, 0.1f); 

                    // Adding direction to orient the object when will be displayed
                    orientations[cpt] = lookDirection;

                    // Update the sphere corresponding 
                    zones[cpt].transform.localPosition = positions[cpt];
                    zones[cpt].SetActive(true);
                    cpt++;
                } 
            } 
        } else {
            if (!canBeUpdated) {
                menu.SetActive(true);
                displayer.SetActive(false);
                MainSceneHandler.SetPositions(GetMostLookedPositions());
                MainSceneHandler.SetOrientations(GetOrientations());
                foreach (GameObject go in zones) {
                    go.SetActive(false);
                }
                canBeUpdated = true;
            }
        }
    }

    private double distance(Vector3 p1, Vector3 p2) {
        return Math.Sqrt(Math.Pow(p2.x-p1.x, 2) + Math.Pow(p2.y-p1.y, 2) + Math.Pow(p2.z-p1.z, 2));
    }

    private float[] GetIntensities() {
        float[] intensities = new float[count];
        for (int i=0; i<count; i++) {
            intensities[i] = properties[i].y;
        }

        return intensities;
    }

    private Vector3[] GetPositions() {
        Vector3[] positionsXYZ = new Vector3[count];
        for (int i=0; i<count; i++) {
            positionsXYZ[i] = new Vector3(positions[i].x, positions[i].y, positions[i].z);
        }

        return positionsXYZ;
    }

    private List<int> GetIndices() {
        int numberFiles = FileListHandler.GetNumberOfChoosenFiles();
        float[] intensities = GetIntensities();
        float[] intensitiesInOrder = GetIntensities();
        List<int> indices = new List<int>();
        List<float> bestIntensities = new List<float>();
        // Sorting array from the larger to the smallest intensity to get numberFiles intensities
        Array.Sort(intensities);
        Array.Reverse(intensities);
        // Only taking the intensities for the number of files to display
        for (int k=0; k<=numberFiles; k++) {
            bestIntensities.Add(intensities[k]);
        }
        // Get index of each intensities we want 
        foreach(float i in bestIntensities) {
            indices.Add(Array.IndexOf(intensitiesInOrder, i));
        }

        return indices;
    }

    public List<Vector3> GetOrientations() {
        List<Vector3> fileOrientations = new List<Vector3>();
        List<int> indexToLook = GetIndices();
        
        foreach(int i in indexToLook) {
            fileOrientations.Add(orientations[i]);
        }

        return fileOrientations;
    }

    public List<Vector3> GetMostLookedPositions() {
        List<Vector3> mostLookedPositions = new List<Vector3>();
        Vector3[] positionsXYZ = GetPositions();
        List<int> indexToLook = GetIndices();

        foreach(int index in indexToLook) {
            mostLookedPositions.Add(positionsXYZ[index]);
        }

        return mostLookedPositions;
    }

    public void ScanEnvironment() {
        cpt = 0;
        timeRemaining = 15.0f;
        canBeUpdated = false;
        menu.SetActive(false);
        displayer.SetActive(true);
        Start();
    }

    public float GetTimeRemaining() {
        return timeRemaining;
    }

}
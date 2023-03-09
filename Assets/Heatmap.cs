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
    public Vector4[] positions;
    public Vector4[] properties;
    private Vector3[] orientations;

    public Material material;
    
    // Number of points in the heatmap (Max is 1024)
    public int count = 100;

    private int cpt = 0;

    // The time to wait before to update the map
    private float nextActionTime = 0.0f;

    // Period of time after each one we add a point in the map 
    private float period = 0.1f;

    // Get menu to not count when user is looking the menu 
    private GameObject menu;

    // Timer of 15 seconds to scan around 
    private float timeRemaining = 30.0f;
 
    void Start ()
    {
        print("Allo");
        positions = new Vector4[count];
        properties = new Vector4[count];
        orientations = new Vector3[count];
        menu = GameObject.Find("BackgroundMenu");

        for (int i = 0; i < positions.Length; i++)
        {
            // pos = (x, y, z)
            // At the launch the pointer is in the center so we initialize all points to 0.0f
            positions[i] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            
            // x = radius 
            // y = intensity
            // radius is 0.4 to be enough large and no too large
            // intensity will correspond to time the user look at this particular point 
            properties[i] = new Vector4(0.4f, 0.0f, 0, 0);

            // Initialise orientations with 0.0f
            orientations[i] = new Vector3(0, 0, 0);
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
                // Vector3 lookAt= CoreServices.InputSystem.EyeGazeProvider.HitPosition;
                Vector3 lookAt = CoreServices.InputSystem.GazeProvider.HitPosition;
                // Direction where the user is looking (don't know which one is the best ?)
                // Vector3 lookDirection = CoreServices.InputSystem.EyeGazeProvider.GazeDirection;
                Vector3 lookDirection = -1.0f * CoreServices.InputSystem.EyeGazeProvider.HitNormal;

                // we put the point where the user is looking in to the map (if not already in)
                Vector4 pointLooked = new Vector4(lookAt.x, lookAt.y, lookAt.z, 0);

                //print(pointLooked);
                bool isAlreadyLooked = Array.Exists(positions, point => 
                    {
                        if (distance(point, pointLooked) < 0.5) {
                            int index = Array.IndexOf(positions, point);
                            properties[index] += new Vector4(0.0f, 0.1f, 0, 0); 
                            return true;
                        } else {
                            return false;
                        }
                    }
                );

                double diag = Math.Sqrt(Math.Pow(2 * menu.GetComponent<Renderer>().bounds.size.x, 2));
                bool isLookingMenu = distance(menu.transform.position, pointLooked) < diag;
                if (!isAlreadyLooked && !isLookingMenu) {
                    print("NotLooked");
                    positions[cpt] = pointLooked;

                    // Update the intensity of this point a little bit 
                    properties[cpt] += new Vector4(0.0f, 0.1f, 0, 0); 

                    // Adding direction to orient the object when will be displayed
                    orientations[cpt] = lookDirection;
                    cpt++;
                }
            } 
        } else {
            //cpt = 0;
            //print("Heatmap calcul done ! Files can be selected ! ");
            menu.SetActive(true);
            DropdownToVisual.SetPositions(GetMostLookedPositions());
            DropdownToVisual.SetOrientations(GetOrientations());
        }

        // NOTE TO DISPLAY FILES : 
        /*
        * Supposing we want to display n files so we do this below n times :
        * 1st : find the max intensity in the properties array
        * 2e : Get the index of the point corresponding to this intensity
        * 3e : get coordinates from index into the positions array 
        * 4e : display the files at these positions 
        */

        material.SetInt("_Points_Length", count);
        material.SetVectorArray("_Points", positions);
        material.SetVectorArray("_Properties", properties);

    }

    private double distance(Vector4 p1, Vector4 p2) {
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
        int numberFiles = DropdownHandler.GetNumberOfChoosenFiles();
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
        Start();
    }

    public float GetTimeRemaining() {
        return timeRemaining;
    }

}
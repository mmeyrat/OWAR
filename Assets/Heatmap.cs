// Code inspired from Alan Zucconi 
// https://www.alanzucconi.com/2016/01/27/arrays-shaders-heatmaps-in-unity3d/#more-2003

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;


public class Heatmap : MonoBehaviour
{   
    // Get menu to not place zones on the menu
    public GameObject menu;
    // A gameobject to detect the cursor presence
    public GameObject displayer;

    // Array which contains all coordinates (x, y, z) of positions where user is looking 
    private Vector3[] positions;
    // Array containing properties about a position looked by the user (radius and intensity of the zone)
    private Vector2[] properties;
    // Array containing orientations (x, y, z) of each zone to place correctly files
    private Vector3[] orientations;
    // Array containing spheres to represent each position looked
    private GameObject[] zones;
    
    // Number of points in the heatmap (Limited to 500 because it's enough)
    private int count = 500;
    // A counter to verify if we have placed the number of "count" points
    private int cpt = 0;
    // The time to wait before to update the map (is incremented during the time)
    private float nextActionTime = 0.0f;
    // Period of time after each one we add and update a point in the map 
    private float period = 0.05f;
    // Timer of 30 seconds to scan around (can be set lower to test)
    private float timeRemaining = 20.0f;
    // A boolean to clean scene and set values only one time in the Update method
    private bool canBeUpdated = false;
    // The number of zones where files will be placed (can be changed)
    private static int numberOfZones = 3; 

    // Start is called before the first frame update
    void Start ()
    {
        positions = new Vector3[count];
        properties = new Vector2[count];
        orientations = new Vector3[count];
        zones = new GameObject[count];

        for (int i = 0; i < positions.Length; i++)
        {
            // Default value updated after
            positions[i] = new Vector3(0.0f, 0.0f, 0.0f);
            
            // radius is 0.05 to be enough large and no too large to instantiate spheres
            // intensity is 0.0f because no points are looked at this moment
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

        // Default values if user do nothing during scan or scan less zones than 3
        float x = -2.0f;
        float defaultIntensity = 0.01f;
        for (int i=0; i<Heatmap.GetNumberOfZones(); i++) {
            properties[i] = new Vector2(0.05f, defaultIntensity);
            positions[i] = new Vector3(x, 0.3f, 1.5f);
            x += 2.0f;
            defaultIntensity++;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (cpt < count && timeRemaining > 0.0f) 
        {
            menu.SetActive(false);
            timeRemaining -= Time.deltaTime;

            if (Time.time > nextActionTime ) 
            { 
                nextActionTime = Time.time + period;

                // Update positions points where the user is looking
                Vector3 pointLooked = CoreServices.InputSystem.GazeProvider.HitPosition;
                
                // Direction where the user is looking 
                Vector3 lookDirection = -1.0f * CoreServices.InputSystem.GazeProvider.HitNormal;

                // we put the point where the user is looking in to the map 
                bool isAlreadyLooked = Array.Exists(positions, point => 
                    {
                        if (Vector3.Distance(point, pointLooked) < 0.25) 
                        {
                            int index = Array.IndexOf(positions, point);
                            properties[index] += new Vector2(0.0f, 0.1f); 
                            if (zones[index].transform.localScale.x < 0.35) 
                            {
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
                bool isLookingMenu = Vector3.Distance(menu.transform.position, pointLooked) < diag;
                if (!isAlreadyLooked && !isLookingMenu) 
                {
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
            // Only done one time in Update with the boolean
            if (!canBeUpdated) 
            {
                menu.SetActive(true);
                displayer.SetActive(false);
                MainSceneHandler.SetPositions(GetMostLookedPositions());
                MainSceneHandler.SetOrientations(GetOrientations());
                foreach (GameObject go in zones) 
                {
                    go.SetActive(false);
                }
                canBeUpdated = true;
            }
        }
    }

    /**
    * Get only intensities from the properties array
    *
    * @return array of each intensities for each positions
    **/
    private float[] GetIntensities() 
    {
        float[] intensities = new float[count];
        for (int i=0; i<count; i++) 
        {
            intensities[i] = properties[i].y;
        }

        return intensities;
    }

    /**
    * Get the numberOfZones highest intensities
    *
    * @return : list of int of numberOfZones indices of the best intensities  
    **/
    private List<int> GetIndices() 
    {
        float[] intensities = GetIntensities();
        float[] intensitiesInOrder = GetIntensities();
        List<int> indices = new List<int>();
        List<float> bestIntensities = new List<float>();
        // Sorting array from the larger to the smallest intensity to get numberofZones intensities
        Array.Sort(intensities);
        Array.Reverse(intensities);
        // Only taking the numberOfZones intensities 
        for (int k=0; k<numberOfZones; k++) 
        {
            bestIntensities.Add(intensities[k]);
        }
        // Get index of each intensities we want 
        foreach(float i in bestIntensities) 
        {
            indices.Add(Array.IndexOf(intensitiesInOrder, i));
        }

        return indices;
    }

    /**
    * Get the concerned positions orientations
    *
    * @return list of Vector3 of orientations for concerned indices
    **/
    public List<Vector3> GetOrientations() 
    {
        List<Vector3> fileOrientations = new List<Vector3>();
        List<int> indexToLook = GetIndices();
        
        foreach(int i in indexToLook) {
            fileOrientations.Add(orientations[i]);
        }

        return fileOrientations;
    }

    /**
    * Get the most looked positions according the number of zones limited
    * 
    * @return a list of Vector3 of the most looked positions 
    **/
    public List<Vector3> GetMostLookedPositions() 
    {
        List<Vector3> mostLookedPositions = new List<Vector3>();
        List<int> indexToLook = GetIndices();

        foreach(int index in indexToLook) 
        {
            mostLookedPositions.Add(positions[index]);
        }

        return mostLookedPositions;
    }

    /**
    * Method to init important variables when the user want to scan his environment again
    **/
    public void ScanEnvironment() 
    {
        cpt = 0;
        timeRemaining = 20.0f;
        canBeUpdated = false;
        menu.SetActive(false);
        displayer.SetActive(true);
        Start();
    }

    /**
    * Get the time remaining to scan the environment
    *
    * @return the time remaining to scan
    **/
    public float GetTimeRemaining() 
    {
        return timeRemaining;
    }

    /**
    * Get the number max of zones where the files are placed
    * 
    * @return the number of zones where files can be placed
    **/ 
    public static int GetNumberOfZones() 
    {
        return numberOfZones;
    }

}
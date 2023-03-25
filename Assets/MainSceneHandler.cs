using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using TMPro;

public class MainSceneHandler : MonoBehaviour
{
    public GameObject FileList;
    public Button button;
    public Text selectedFiles;
    public GameObject warning;
    public GameObject mixedRealityPlayspace; 

    private int minChoosenFiles = 0;
    private string[] imageExtensions = { ".jpg", ".jpeg", ".png" };
    private string textExtension = ".txt";

    // List of position for each file
    private static List<Vector3> positionsFiles;
    // List of orientation for each file
    private static List<Vector3> orientationsFiles;
    // Time when the user click on visualize
    private static float timeFilesSelected;
    // A little offset to apply a correct force on files when they appear
    private float offsetDisplay = 0.1f;
    // A little sphere representing the center of the most looked zone
    private GameObject gravityCenter;
    
    /**
    * Add the selecetd files in a text preview
    *
    * @param label : Text object containing the file's name 
    **/
    public void FileSelector(Text label) 
    {
        string file = label.text;

        FileListHandler.SetToChoosen(file);

        if (FileListHandler.IsFileChoosen(file))
        {
            selectedFiles.text += $"\n• {file}";
        } 
        else 
        {
            selectedFiles.text = selectedFiles.text.Replace($"\n• {file}", "");
        }
    }

    /**
    * Change the scene to display the files
    **/
    public void Visualize() 
    {
        if (FileListHandler.GetNumberOfChoosenFiles() > 0) 
        {
            string[] files = FileListHandler.GetFiles();
            int numberOfZones = Heatmap.GetNumberOfZones();
            int indexZone = 0;
            int counterFilePerZone = 0;
            int nbFilesMaxPerZone = FileListHandler.GetNumberOfChoosenFiles()/numberOfZones;

            foreach (string f in files) 
            {
                if (FileListHandler.IsFileChoosen(f)) 
                {
                    if (indexZone < 2 && counterFilePerZone == nbFilesMaxPerZone) {
                        indexZone++;
                        counterFilePerZone = 0;
                        offsetDisplay = 0.1f;
                    }

                    Vector3 center = positionsFiles[indexZone];
                    Vector3 orientation = orientationsFiles[indexZone];

                    if (Array.IndexOf(imageExtensions, Path.GetExtension(f)) >= 0) 
                    {
                        GameObject imagePrefab = Instantiate(Resources.Load("ImagePrefab")) as GameObject;
                        imagePrefab.transform.localPosition = new Vector3(center.x + offsetDisplay, center.y + offsetDisplay, center.z);
                        imagePrefab.transform.rotation = Quaternion.LookRotation(orientation);
                        mixedRealityPlayspace.GetComponent<ApplyForces>().AddObj(imagePrefab, indexZone);
                        counterFilePerZone++;
                        
                        // Link to close button
                        imagePrefab.GetComponent<Close>().SetObj(imagePrefab);
                        imagePrefab.GetComponent<Close>().SetIndexZone(indexZone);

                        imagePrefab.GetComponent<DisplayImage>().SetImageObject(imagePrefab.transform.GetChild(0).GetChild(0).GetComponent<RawImage>());
                        imagePrefab.GetComponent<DisplayImage>().SetFileName(Path.Combine(FileListHandler.GetPath(), f));

                        offsetDisplay += 0.15f;
                        selectedFiles.text = selectedFiles.text.Replace($"\n• {f}", "");
                    }
                    else if (f.EndsWith(textExtension)) 
                    {   
                        GameObject textPrefab = Instantiate(Resources.Load("TextPrefab")) as GameObject;

                        // Setting position according informations obtained with the heatmap
                        textPrefab.transform.localPosition = new Vector3(center.x + offsetDisplay, center.y + offsetDisplay, center.z);
                        textPrefab.transform.rotation = Quaternion.LookRotation(orientation);
                        mixedRealityPlayspace.GetComponent<ApplyForces>().AddObj(textPrefab, indexZone);
                        counterFilePerZone++;

                        // Link to close button
                        textPrefab.GetComponent<Close>().SetObj(textPrefab);
                        textPrefab.GetComponent<Close>().SetIndexZone(indexZone);

                        // Link to next page button
                        textPrefab.GetComponent<ChangePage>().SetObj(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        
                        textPrefab.GetComponent<DisplayText>().SetTextObject(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        textPrefab.GetComponent<DisplayText>().SetFileName(Path.Combine(FileListHandler.GetPath(), f));

                        selectedFiles.text = selectedFiles.text.Replace($"\n• {f}", "");
                        offsetDisplay += 0.15f;
                    } 
                }
            }

            offsetDisplay = 0.1f;
            timeFilesSelected = Time.time;

            // Init all centers of gravity for objects files
            InitGravityCenters();
            
            mixedRealityPlayspace.GetComponent<ApplyForces>().InitMovements();

        } else {
            warning.SetActive(true);
            warning.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            warning.GetComponent<UnityEngine.UI.Text>().CrossFadeAlpha(0.0f, 2.0f, false);
        }

        // Untoggle visualized file in the list
        GameObject panelListComp = FileList.transform.GetChild(0).gameObject;

        for (int i = 0; i < panelListComp.transform.childCount; i++) 
        {
            GameObject currentChild = panelListComp.transform.GetChild(i).gameObject;
            //Check if it's an item which corresponds to a selected file 
            string fileName = currentChild.transform.GetChild(1).GetChild(1).GetComponent<Text>().text;

            if (FileListHandler.IsFileChoosen(fileName))
            {     
                currentChild.transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
            }
        }
    }

    /**
    * Put a little sphere where the user has looked the most
    **/
    private void InitGravityCenters() 
    {
        for (int i=0; i < Heatmap.GetNumberOfZones(); i++) 
        {
            gravityCenter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            gravityCenter.name = "GravityCenter";
            gravityCenter.transform.position = positionsFiles[i];
            gravityCenter.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        }
    }

    /**
    * Method to set the scene and important variables
    **/
    public void StartScanningEnvironment() 
    {
        mixedRealityPlayspace.GetComponent<ApplyForces>().RemoveAllObjects();
        mixedRealityPlayspace.GetComponent<Heatmap>().ScanEnvironment();
    }

    /**
    * Set the positions of game objects representing each files
    * @param positionsXYZ : list of vector of positions looked
    **/
    public static void SetPositions(List<Vector3> positionsXYZ) 
    {
        positionsFiles = positionsXYZ;
    }

    /** 
    * Set the orientations for each positions where the user has the most looked
    * @param orientations : list of Vector3 of orientations for each position 
    **/
    public static void SetOrientations(List<Vector3> orientations) 
    {
        orientationsFiles = orientations;
    }

    /** 
    * Get the time when the user has clicked on the "visualize" button
    *
    * @return a float which correspond to the time in second from the launch of the application
    **/
    public static float GetTimeFilesSelected() 
    {
        return timeFilesSelected;
    }

    /** 
    * Get the position along axis x and y of the most looked positions
    *
    * @return a Vector2 which is the most looked position along axis x and y depending about the zone concerned
    **/
    public static Vector2 GetGravityCenterPosition(int indexZone) 
    {
        return new Vector2(positionsFiles[indexZone].x, positionsFiles[indexZone].y);
    }
}

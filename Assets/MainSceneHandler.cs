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

    private static List<Vector3> positionsFiles;
    private static List<Vector3> orientationsFiles;
    private static float timeFilesSelected;
    private float offsetDisplay = 0.1f;
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
            Vector3 bestPosition = GetTheMostLookedPosition();
            Vector3 orientation = GetOrientationOfMostLookedPosition();

            foreach (string f in files) 
            {
                if (FileListHandler.IsFileChoosen(f)) 
                {
                    if (Array.IndexOf(imageExtensions, Path.GetExtension(f)) >= 0) 
                    {
                        GameObject imagePrefab = Instantiate(Resources.Load("ImagePrefab")) as GameObject;
                        imagePrefab.transform.localPosition = new Vector3(bestPosition.x + offsetDisplay, bestPosition.y + offsetDisplay, bestPosition.z);
                        imagePrefab.transform.rotation = Quaternion.LookRotation(orientation);
                        mixedRealityPlayspace.GetComponent<ApplyForces>().AddObj(imagePrefab);
                        
                        // Link to close button
                        imagePrefab.GetComponent<Close>().SetObj(imagePrefab);
                        
                        imagePrefab.GetComponent<DisplayImage>().SetImageObject(imagePrefab.transform.GetChild(0).GetChild(0).GetComponent<RawImage>());
                        imagePrefab.GetComponent<DisplayImage>().SetFileName(Path.Combine(FileListHandler.GetPath(), f));

                        offsetDisplay += 0.1f;
                        selectedFiles.text = selectedFiles.text.Replace($"\n• {f}", "");
                    }
                    else if (f.EndsWith(textExtension)) 
                    {   
                        GameObject textPrefab = Instantiate(Resources.Load("TextPrefab")) as GameObject;

                        // Setting position according informations obtained with the heatmap
                        textPrefab.transform.localPosition = new Vector3(bestPosition.x + offsetDisplay, bestPosition.y - offsetDisplay, bestPosition.z);
                        textPrefab.transform.rotation = Quaternion.LookRotation(orientation);
                        mixedRealityPlayspace.GetComponent<ApplyForces>().AddObj(textPrefab);

                        // Link to close button
                        textPrefab.GetComponent<Close>().SetObj(textPrefab);
                        // Link to next page button
                        textPrefab.GetComponent<ChangePage>().SetObj(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        
                        textPrefab.GetComponent<DisplayText>().SetTextObject(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        textPrefab.GetComponent<DisplayText>().SetFileName(Path.Combine(FileListHandler.GetPath(), f));

                        selectedFiles.text = selectedFiles.text.Replace($"\n• {f}", "");
                        offsetDisplay += 0.1f;
                    } 
                }
            }

            offsetDisplay = 0.1f;
            timeFilesSelected = Time.time;

            // Init a center of gravity for objects files
            InitGravityCenter();
            
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

    private void InitGravityCenter() 
    {
        gravityCenter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gravityCenter.name = "GravityCenter";
        gravityCenter.transform.position = positionsFiles[0];
        gravityCenter.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
    }

    public void StartScanningEnvironment() 
    {
        mixedRealityPlayspace.GetComponent<ApplyForces>().RemoveAllObjects();
        mixedRealityPlayspace.GetComponent<Heatmap>().ScanEnvironment();
    }

    public static void SetPositions(List<Vector3> positionsXYZ) 
    {
        positionsFiles = positionsXYZ;
    }

    private Vector3 GetTheMostLookedPosition() {
        return positionsFiles[0];
    }

    private Vector3 GetOrientationOfMostLookedPosition() {
        return orientationsFiles[0];
    }

    public static void SetOrientations(List<Vector3> orientations) 
    {
        orientationsFiles = orientations;
    }

    public static float GetTimeFilesSelected() 
    {
        return timeFilesSelected;
    }

    public static Vector2 GetGravityCenterPosition() 
    {
        return new Vector2(positionsFiles[0].x, positionsFiles[0].y);
    }
}

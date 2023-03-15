using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using TMPro;

public class DropdownToVisual : MonoBehaviour
{
    public Dropdown dropdownList;
    public Button button;
    public Text selectedFiles;
    public GameObject warning;
    public GameObject mixedRealityPlayspace; 

    private static List<Vector3> positionsFiles;
    private static List<Vector3> orientationsFiles;
    private static float timeFilesSelected;
    private float offsetDisplay = 0.01f;
    private GameObject heatmapVisualizer;
    private GameObject gravityCenter;

    void Start() {
        heatmapVisualizer = GameObject.Find("HeatmapVisualizer");
    }

    /**
    * Add the selecetd files in a text preview
    **/
    public void FileSelector() 
    {
        int index = dropdownList.value;
        string file = dropdownList.options[index].text;

        DropdownHandler.SetToChoosen(file);

        if (DropdownHandler.IsFileChoosen(file)) 
        {
            selectedFiles.text += $"\n - {file}";
        } 
        else 
        {
            selectedFiles.text = selectedFiles.text.Replace($"\n - {file}", "");
        }
    }

    /**
    * Change the scene to display the files
    **/
    public void Visualize() 
    {
        if (DropdownHandler.GetNumberOfChoosenFiles() > 0) 
        {
            string[] files = DropdownHandler.GetFiles();
            int fileIndex = 0;

            foreach (string f in files) 
            {
                if (f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png")) 
                {
                    if (DropdownHandler.IsFileChoosen(f)) 
                    {
                        GameObject imagePrefab = Instantiate(Resources.Load("ImagePrefab")) as GameObject;
                        Rigidbody body = imagePrefab.AddComponent<Rigidbody>();
                        body.isKinematic = true;
                        body.useGravity = false;
                        imagePrefab.transform.localPosition = new Vector3(positionsFiles[fileIndex].x + offsetDisplay, positionsFiles[fileIndex].y - offsetDisplay, positionsFiles[fileIndex].z);
                        imagePrefab.transform.rotation = Quaternion.LookRotation(orientationsFiles[fileIndex]);
                        mixedRealityPlayspace.GetComponent<ApplyForces>().AddObj(imagePrefab);
                        
                        // Link to close button
                        imagePrefab.GetComponent<Close>().SetObj(imagePrefab);
                        
                        imagePrefab.GetComponent<DisplayImage>().SetImageObject(imagePrefab.transform.GetChild(0).GetChild(0).GetComponent<RawImage>());
                        imagePrefab.GetComponent<DisplayImage>().SetFileName(Path.Combine(DropdownHandler.GetPath(), f));

                        selectedFiles.text = selectedFiles.text.Replace($"\n - {f}", "");
                        DropdownHandler.SetToChoosen(f);
                        //fileIndex++;
                        offsetDisplay += 0.01f;
                    } 
                }
                else if (f.EndsWith(".txt")) 
                {   
                    if (DropdownHandler.IsFileChoosen(f)) 
                    {
                        GameObject textPrefab = Instantiate(Resources.Load("TextPrefab")) as GameObject;
                        Rigidbody body = textPrefab.AddComponent<Rigidbody>();
                        body.isKinematic = true;
                        body.useGravity = false;
                        // Setting position according informations obtained with the heatmap
                        textPrefab.transform.localPosition = new Vector3(positionsFiles[fileIndex].x + offsetDisplay, positionsFiles[fileIndex].y - offsetDisplay, positionsFiles[fileIndex].z);
                        textPrefab.transform.rotation = Quaternion.LookRotation(orientationsFiles[fileIndex]);
                        mixedRealityPlayspace.GetComponent<ApplyForces>().AddObj(textPrefab);

                        // Link to close button
                        textPrefab.GetComponent<Close>().SetObj(textPrefab);
                        // Link to next page button
                        textPrefab.GetComponent<ChangePage>().SetObj(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        
                        textPrefab.GetComponent<ReadText>().SetTextObject(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        textPrefab.GetComponent<ReadText>().SetFileName(Path.Combine(DropdownHandler.GetPath(), f));

                        selectedFiles.text = selectedFiles.text.Replace($"\n - {f}", "");
                        DropdownHandler.SetToChoosen(f);
                        //fileIndex++;
                        offsetDisplay += 0.01f;
                    }
                }
            }
            offsetDisplay = 0.01f;
            timeFilesSelected = Time.time;

            // Init a center of gravity for objects files
            if (GameObject.Find("GravityCenter") == null)
                InitGravityCenter();

            mixedRealityPlayspace.GetComponent<ApplyForces>().InitMovements();
            if (heatmapVisualizer != null)
                heatmapVisualizer.SetActive(false);
        } else {
            warning.SetActive(true);
            warning.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            warning.GetComponent<UnityEngine.UI.Text>().CrossFadeAlpha(0.0f, 2.0f, false);
        }
    }

    private void InitGravityCenter() {
        gravityCenter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gravityCenter.name = "GravityCenter";
        gravityCenter.transform.position = positionsFiles[0];
        gravityCenter.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        gravityCenter.GetComponent<SphereCollider>().isTrigger = true;
        gravityCenter.GetComponent<SphereCollider>().name = "CenterCollider";
        Rigidbody centerRigidbody = gravityCenter.AddComponent<Rigidbody>();
        centerRigidbody.isKinematic = true;
        centerRigidbody.useGravity = false;
    }

    public void StartScanningEnvironment() {
        heatmapVisualizer.SetActive(true);
        mixedRealityPlayspace.GetComponent<ApplyForces>().RemoveAllObjects();
        mixedRealityPlayspace.GetComponent<Heatmap>().ScanEnvironment();
    }

    public static void SetPositions(List<Vector3> positionsXYZ) {
        positionsFiles = positionsXYZ;
    }

    public static void SetOrientations(List<Vector3> orientations) {
        orientationsFiles = orientations;
    }

    public static float GetTimeFilesSelected() {
        return timeFilesSelected;
    }

    public static Vector3 GetGravityCenterPosition() {
        return positionsFiles[0];
    }

}

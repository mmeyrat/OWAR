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
            float offsetImage = 0.2f;
            float offsetImageIncrement = 3.0f;
            float offsetText = - 0.2f;
            float offsetTextIncrement = 2.1f;
            string[] files = DropdownHandler.GetFiles();
            
            foreach (string f in files) 
            {
                if (f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png")) 
                {
                    if (DropdownHandler.IsFileChoosen(f)) 
                    {
                        GameObject imagePrefab = Instantiate(Resources.Load("ImagePrefab")) as GameObject;
                        
                        string[] tags = new string[] { "Screen", "Screen" };
                        imagePrefab.tag = tags[UnityEngine.Random.Range (0, tags.Length)];

                        imagePrefab.transform.position = GetPositionBasedOnTag(imagePrefab);
                        imagePrefab = RotateBasedOnTag(imagePrefab);

                        // Link to close button
                        imagePrefab.GetComponent<Close>().SetObj(imagePrefab);
                        
                        imagePrefab.GetComponent<DisplayImage>().SetImageObject(imagePrefab.transform.GetChild(0).GetChild(0).GetComponent<RawImage>());
                        imagePrefab.GetComponent<DisplayImage>().SetFileName(Path.Combine(DropdownHandler.GetPath(), f));
                        imagePrefab.GetComponent<DisplayImage>().SetPoseX(offsetImage);

                        offsetImage *= offsetImageIncrement;

                        selectedFiles.text = selectedFiles.text.Replace($"\n - {f}", "");
                        DropdownHandler.SetToChoosen(f);
                    } 
                }
                else if (f.EndsWith(".txt")) 
                {   
                    if (DropdownHandler.IsFileChoosen(f)) 
                    {
                        GameObject textPrefab = Instantiate(Resources.Load("TextPrefab")) as GameObject;
                        //textPrefab.transform.localPosition = new Vector3(offsetText, textPrefab.transform.localPosition.y, textPrefab.transform.localPosition.z);
                        
                        // Link to close button
                        textPrefab.GetComponent<Close>().SetObj(textPrefab);
                        // Link to next page button
                        textPrefab.GetComponent<ChangePage>().SetObj(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        
                        textPrefab.GetComponent<ReadText>().SetTextObject(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        textPrefab.GetComponent<ReadText>().SetFileName(Path.Combine(DropdownHandler.GetPath(), f));

                        offsetText *= offsetTextIncrement;

                        selectedFiles.text = selectedFiles.text.Replace($"\n - {f}", "");
                        DropdownHandler.SetToChoosen(f);
                    }
                }
            }
        } else {
            warning.SetActive(true);
            warning.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            warning.GetComponent<UnityEngine.UI.Text>().CrossFadeAlpha(0.0f, 2.0f, false);
        }
    }

    public Vector3 GetPositionBasedOnTag(GameObject imagePrefab)
    {
        int areaId = GetNearestAreaFromTag(imagePrefab.tag);
        Vector3 v3 = new Vector3(0,0,0);

        if (areaId >= 0)
        {
            TagArea currentTagArea = TagSceneHandler.GetTagAreaList()[areaId];

            if (currentTagArea.GetSlotAvailability(0))
            {
                currentTagArea.SetSlotAvailability(0, false);
                float tmp = currentTagArea.GetPosition().x - currentTagArea.GetScale().x / 2 - imagePrefab.transform.localScale.x;
                v3 = new Vector3(tmp, currentTagArea.GetPosition().y, currentTagArea.GetPosition().z);
            } 
            else if (currentTagArea.GetSlotAvailability(1))
            {
                currentTagArea.SetSlotAvailability(1, false);
                float tmp = currentTagArea.GetPosition().y + currentTagArea.GetScale().y / 2 + imagePrefab.transform.localScale.y;
                v3 = new Vector3(currentTagArea.GetPosition().x, tmp, currentTagArea.GetPosition().z);
            } 
            else if (currentTagArea.GetSlotAvailability(2))
            {
                currentTagArea.SetSlotAvailability(2, false);
                float tmp = currentTagArea.GetPosition().x + currentTagArea.GetScale().x / 2 + imagePrefab.transform.localScale.x;
                v3 = new Vector3(tmp, currentTagArea.GetPosition().y, currentTagArea.GetPosition().z);
            }
        }

        Debug.Log(areaId);
        return v3;
    }

    public int GetNearestAreaFromTag(string tag)
    {
        int id = -1;
        float minDist = 1000;
        GameObject camera = GameObject.Find("Main Camera");

        for (int i = 0; i < TagSceneHandler.GetTagAreaList().Count; i++)
        {
            if (TagSceneHandler.GetTagAreaList()[i].GetTag() == tag)
            {
                float currentDist = Vector3.Distance(TagSceneHandler.GetTagAreaList()[i].GetPosition(), camera.transform.position);
            
                if (currentDist < minDist)
                {
                    minDist = currentDist;
                    id = i;
                }
            }
        }

        return id;
    }

    public GameObject RotateBasedOnTag(GameObject imagePrefab)
    {
        int areaId = GetNearestAreaFromTag(imagePrefab.tag);

        if (areaId >= 0)
        {
            GameObject rotationParent = new GameObject("RotationParent");

            rotationParent.transform.position = TagSceneHandler.GetTagAreaList()[areaId].GetPosition();
            rotationParent.transform.localScale = TagSceneHandler.GetTagAreaList()[areaId].GetScale();

            imagePrefab.transform.parent = rotationParent.transform;

            rotationParent.transform.localRotation = TagSceneHandler.GetTagAreaList()[areaId].GetRotation();
        }

        return imagePrefab;
    }
}

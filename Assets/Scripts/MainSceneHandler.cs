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

    private GameObject mainCamera;

    private int minChoosenFiles = 0;
    private int maxSlots = 4; // Number of slots per tag area
    private int minTagAreaId = 0;
    private float dist = 1.0f; // Distance between object and camera
    private float fileWithNoAreaOffset = 0.05f; // Gap between files placed with no tag area 
    private float posDistOffset = 2.0f; // For slot positionning
    private string[] imageExtensions = { ".jpg", ".jpeg", ".png" };
    private string textExtension = ".txt";
    
    /** 
    * Start is called before the first frame update
    **/
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
    }

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
        if (FileListHandler.GetNumberOfChoosenFiles() > minChoosenFiles) 
        {
            string[] files = FileListHandler.GetFiles();
            FileTagList ftl = FileTagList.GetFileTagList();
            float nbOfFilesWithNoArea = 0.0f;
            
            foreach (string f in files) 
            {
                if (FileListHandler.IsFileChoosen(f)) 
                {   
                    GameObject prefab;
                    string tag = "[Untagged]";
                    
                    if (ftl.fileList.Contains(f))
                    {
                        tag = ftl.tagList[ftl.fileList.IndexOf(f)];
                    }

                    int areaId = GetNearestAreaFromTag(tag);

                    if (Array.IndexOf(imageExtensions, Path.GetExtension(f)) >= 0) 
                    {  
                        prefab = Instantiate(Resources.Load("ImagePrefab")) as GameObject;
                        
                        prefab.GetComponent<DisplayImage>().SetImageObject(prefab.transform.GetChild(0).GetChild(0).GetComponent<RawImage>());
                        prefab.GetComponent<DisplayImage>().SetFileName(Path.Combine(FileListHandler.GetPath(), f));
                    }
                    else if (f.EndsWith(textExtension)) 
                    {       
                        prefab = Instantiate(Resources.Load("TextPrefab")) as GameObject;
                        
                        prefab.GetComponent<DisplayText>().SetTextObject(prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        prefab.GetComponent<DisplayText>().SetFileName(Path.Combine(FileListHandler.GetPath(), f));
                        // Link to next & previous page buttons
                        prefab.GetComponent<ChangePage>().SetObj(prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                    }
                    else 
                    {
                        prefab = new GameObject();
                    }

                    if (areaId >= minTagAreaId)
                    {
                        int availableSlot = CheckAvailableSlot(tag);
                        
                        prefab.transform.position = GetPositionBasedOnTag(prefab, availableSlot, tag);
                        prefab = RotateBasedOnTag(prefab, tag);
                        TagSceneHandler.GetTagAreaList()[areaId].SetSlotAvailability(availableSlot, false);
                        
                        prefab.GetComponent<PrefabData>().SetTagAreaSlotId(availableSlot);
                    }    
                    else 
                    {
                        prefab.transform.position = mainCamera.transform.position + mainCamera.transform.forward * (dist + nbOfFilesWithNoArea * fileWithNoAreaOffset);
                        prefab.transform.rotation = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0);
                        nbOfFilesWithNoArea++;
                    }

                    // Link to close button
                    prefab.GetComponent<Close>().SetObj(prefab);
                    prefab.GetComponent<PrefabData>().SetTagAreaId(areaId);

                    selectedFiles.text = selectedFiles.text.Replace($"\n• {f}", "");
                }
            }
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
    * Check every tag area to find the nearest one to the camera for a given tag
    *
    * @param tag : the tag to check
    * @return the area id
    **/
    public int GetNearestAreaFromTag(string tag)
    {
        int id = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < TagSceneHandler.GetTagAreaList().Count; i++)
        {
            TagArea currentTagArea = TagSceneHandler.GetTagAreaList()[i];

            if (currentTagArea.GetTag() == tag && 
                (currentTagArea.GetSlotAvailability(0) || currentTagArea.GetSlotAvailability(1) || 
                 currentTagArea.GetSlotAvailability(2) || currentTagArea.GetSlotAvailability(3)))
            {
                float currentDist = Vector3.Distance(currentTagArea.GetPosition(), mainCamera.transform.position);
            
                if (currentDist < minDist)
                {
                    minDist = currentDist;
                    id = i;
                }
            }
        }

        return id;
    }

    /**
    * Compute the correct position to place the prefab based on its slot
    *
    * @param prefab : the prefab to place
    * @param slot : the prefab's asigned slot
    * @param tag : the prefab's tag
    * @return the position
    **/
    public Vector3 GetPositionBasedOnTag(GameObject prefab, int slot, string tag)
    {
        Vector3 position = new Vector3(0, 0, 0);
        int areaId = GetNearestAreaFromTag(tag);
        float slotPos = 0.0f;

        if (areaId >= minTagAreaId)
        {
            TagArea currentTagArea = TagSceneHandler.GetTagAreaList()[areaId];
            float width = prefab.GetComponent<BoxCollider>().size[0];
            float height = prefab.GetComponent<BoxCollider>().size[1];

            switch(slot)
            {
                case 0:
                    slotPos = currentTagArea.GetPosition().x - currentTagArea.GetScale().x / posDistOffset - prefab.transform.localScale.x * width;
                    position = new Vector3(slotPos, currentTagArea.GetPosition().y, currentTagArea.GetPosition().z);
                    break;
                case 1:
                    slotPos = currentTagArea.GetPosition().y + currentTagArea.GetScale().y / posDistOffset + prefab.transform.localScale.y * height;
                    position = new Vector3(currentTagArea.GetPosition().x, slotPos, currentTagArea.GetPosition().z);
                    break;
                case 2:
                    slotPos = currentTagArea.GetPosition().x + currentTagArea.GetScale().x / posDistOffset + prefab.transform.localScale.x * width;
                    position = new Vector3(slotPos, currentTagArea.GetPosition().y, currentTagArea.GetPosition().z);
                    break;
                case 3:
                    slotPos = currentTagArea.GetPosition().y - currentTagArea.GetScale().y / posDistOffset - prefab.transform.localScale.y * height;
                    position = new Vector3(currentTagArea.GetPosition().x, slotPos, currentTagArea.GetPosition().z);
                    break;
                default:
                    break;
            }
        }

        return position;
    }

    /**
    * Add a parent to the prefab to perform a rotation based on the specified tag area
    *
    * @param prefab : the prefab to rotate
    * @param tag : the prefab's tag
    * @return the rotated prefab
    **/
    public GameObject RotateBasedOnTag(GameObject prefab, string tag)
    {
        int areaId = GetNearestAreaFromTag(tag);

        if (areaId >= minTagAreaId)
        {
            GameObject rotationParent = new GameObject("RotationParent");

            rotationParent.transform.position = TagSceneHandler.GetTagAreaList()[areaId].GetPosition();
            rotationParent.transform.localScale = TagSceneHandler.GetTagAreaList()[areaId].GetScale();

            prefab.transform.SetParent(rotationParent.transform);

            rotationParent.transform.localRotation = TagSceneHandler.GetTagAreaList()[areaId].GetRotation();
        }

        return prefab;
    }

    /**
    * Check if a slot is available for the nearest tag area
    *
    * @param tag : the tag area's tag
    * @return the available slot
    **/
    public int CheckAvailableSlot(string tag)
    {
        int noSlotId = -1;
        int areaId = GetNearestAreaFromTag(tag);

        if (areaId >= minTagAreaId)
        {
            TagArea currentTagArea = TagSceneHandler.GetTagAreaList()[areaId];
            
            for (int i = 0 ; i < maxSlots; i++)
            {
                if (currentTagArea.GetSlotAvailability(i))
                {
                    return i;
                }
            }
        }

        return noSlotId;
    }

    /**
    * Change the current scene to the tag scene
    **/
    public void SwitchToTagScene()
    {
        SceneManager.LoadScene("TagScene");
    }
}

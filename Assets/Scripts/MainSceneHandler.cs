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
            selectedFiles.text += $"\n-{file}";
        } 
        else 
        {
            selectedFiles.text = selectedFiles.text.Replace($"\n-{file}", "");
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
            string[] tags = new string[] { "Screen", "Table" };
            
            foreach (string f in files) 
            {
                if (f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png")) 
                {
                    string tag = tags[UnityEngine.Random.Range (0, tags.Length)];
                    int areaId = GetNearestAreaFromTag(tag);

                    if (FileListHandler.IsFileChoosen(f) && areaId >= 0) 
                    {
                        GameObject imagePrefab = Instantiate(Resources.Load("ImagePrefab")) as GameObject;
                        imagePrefab.tag = tag;
                        
                        int availableSlot = CheckAvailableSlot(imagePrefab);
                        imagePrefab.transform.position = GetPositionBasedOnTag(imagePrefab, availableSlot);
                        imagePrefab = RotateBasedOnTag(imagePrefab);
                        imagePrefab.GetComponent<PrefabData>().SetTagAreaId(areaId);
                        imagePrefab.GetComponent<PrefabData>().SetTagAreaSlotId(availableSlot);
                        TagSceneHandler.GetTagAreaList()[areaId].SetSlotAvailability(availableSlot, false);

                        // Link to close button
                        imagePrefab.GetComponent<Close>().SetObj(imagePrefab);
                        
                        imagePrefab.GetComponent<DisplayImage>().SetImageObject(imagePrefab.transform.GetChild(0).GetChild(0).GetComponent<RawImage>());
                        imagePrefab.GetComponent<DisplayImage>().SetFileName(Path.Combine(FileListHandler.GetPath(), f));

                        selectedFiles.text = selectedFiles.text.Replace($"\n-{f}", "");
                        //FileListHandler.SetToChoosen(f);

                        TagArea ta = TagSceneHandler.GetTagAreaList()[areaId];
                        GameObject tagAreaPrefab = Instantiate(Resources.Load("TagAreaPrefab")) as GameObject;
                        tagAreaPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = ta.GetTag();
                        tagAreaPrefab.tag = ta.GetTag();

                        tagAreaPrefab.transform.position = ta.GetPosition();
                        tagAreaPrefab.transform.localScale = ta.GetScale();
                        tagAreaPrefab.transform.localRotation = ta.GetRotation();
                    } 
                }
                else if (f.EndsWith(".txt")) 
                {   
                    string tag = tags[UnityEngine.Random.Range (0, tags.Length)];
                    int areaId = GetNearestAreaFromTag(tag);

                    if (FileListHandler.IsFileChoosen(f) && areaId >= 0) 
                    {
                        GameObject textPrefab = Instantiate(Resources.Load("TextPrefab")) as GameObject;
                        textPrefab.tag = tag;
                        
                        int availableSlot = CheckAvailableSlot(textPrefab);
                        textPrefab.transform.position = GetPositionBasedOnTag(textPrefab, availableSlot);
                        textPrefab = RotateBasedOnTag(textPrefab);
                        textPrefab.GetComponent<PrefabData>().SetTagAreaId(areaId);
                        textPrefab.GetComponent<PrefabData>().SetTagAreaSlotId(availableSlot);
                        TagSceneHandler.GetTagAreaList()[areaId].SetSlotAvailability(availableSlot, false);

                        // Link to close button
                        textPrefab.GetComponent<Close>().SetObj(textPrefab);
                        // Link to next page button
                        textPrefab.GetComponent<ChangePage>().SetObj(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        
                        textPrefab.GetComponent<ReadText>().SetTextObject(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        textPrefab.GetComponent<ReadText>().SetFileName(Path.Combine(FileListHandler.GetPath(), f));

                        selectedFiles.text = selectedFiles.text.Replace($"\n-{f}", "");
                        //FileListHandler.SetToChoosen(f);
                    }
                }
            }
        } else {
            warning.SetActive(true);
            warning.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
            warning.GetComponent<UnityEngine.UI.Text>().CrossFadeAlpha(0.0f, 2.0f, false);
        }

        // Untoggle visualized file in the list
        GameObject panelListComponent = FileList.transform.GetChild(0).gameObject;

        for (int i = 1; i < panelListComponent.transform.childCount; i++) // Start at 1 to avoid the disabled template item
        {
            GameObject currentChild = panelListComponent.transform.GetChild(i).gameObject;

            //Check if it's an item which corresponds to a selected file 
            Text currentChildText = currentChild.transform.GetChild(2).GetComponentInChildren<Text>();
            string filename = currentChildText.text;

            if (FileListHandler.IsFileChoosen(filename))
            {     
                Toggle currentChildToggle = (Toggle)currentChild.transform.Find("Toggle").GetComponent<Toggle>();
                currentChildToggle.isOn = false;
            }
        }
    }

    public int GetNearestAreaFromTag(string tag)
    {
        int id = -1;
        float minDist = 1000;
        GameObject camera = GameObject.Find("Main Camera");

        for (int i = 0; i < TagSceneHandler.GetTagAreaList().Count; i++)
        {
            TagArea currentTagArea = TagSceneHandler.GetTagAreaList()[i];

            if (currentTagArea.GetTag() == tag && (currentTagArea.GetSlotAvailability(0) || currentTagArea.GetSlotAvailability(1) || currentTagArea.GetSlotAvailability(2) || currentTagArea.GetSlotAvailability(3)))
            {
                float currentDist = Vector3.Distance(currentTagArea.GetPosition(), camera.transform.position);
            
                if (currentDist < minDist)
                {
                    minDist = currentDist;
                    id = i;
                }
            }
        }

        return id;
    }

    public Vector3 GetPositionBasedOnTag(GameObject prefab, int slot)
    {
        Vector3 position = new Vector3(0,0,0);
        int areaId = GetNearestAreaFromTag(prefab.tag);
        float slotPos = 0.0f;
        float posDistOffset = 2.0f;

        if (areaId >= 0)
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

    public GameObject RotateBasedOnTag(GameObject prefab)
    {
        int areaId = GetNearestAreaFromTag(prefab.tag);

        if (areaId >= 0)
        {
            GameObject rotationParent = new GameObject("RotationParent");

            rotationParent.transform.position = TagSceneHandler.GetTagAreaList()[areaId].GetPosition();
            rotationParent.transform.localScale = TagSceneHandler.GetTagAreaList()[areaId].GetScale();

            prefab.transform.parent = rotationParent.transform;

            rotationParent.transform.localRotation = TagSceneHandler.GetTagAreaList()[areaId].GetRotation();
        }

        return prefab;
    }

    public int CheckAvailableSlot(GameObject prefab)
    {
        int maxSlots = 4;
        int areaId = GetNearestAreaFromTag(prefab.tag);

        if (areaId >= 0)
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

        return -1;
    }

    /**
    * Change the current scene to the tag scene
    **/
    public void SwitchToTagScene()
    {
        SceneManager.LoadScene("TagScene");
    }
}

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
            StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/FileTagList.json");
            FileTagList ftl = JsonUtility.FromJson<FileTagList>(reader.ReadToEnd());
            
            foreach (string f in files) 
            {
                string tag = "[Untagged]";
                
                if (ftl.fileList.Contains(f))
                {
                    tag = ftl.tagList[ftl.fileList.IndexOf(f)];
                }

                int areaId = GetNearestAreaFromTag(tag);

                if (FileListHandler.IsFileChoosen(f) && areaId >= 0) 
                {   
                    if (f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png")) 
                    {  
                        GameObject prefab = Instantiate(Resources.Load("ImagePrefab")) as GameObject;
                        prefab = PreparePrefab(prefab, tag, areaId);
                        
                        prefab.GetComponent<DisplayImage>().SetImageObject(prefab.transform.GetChild(0).GetChild(0).GetComponent<RawImage>());
                        prefab.GetComponent<DisplayImage>().SetFileName(Path.Combine(FileListHandler.GetPath(), f));
                    }
                    else if (f.EndsWith(".txt")) 
                    {       
                        GameObject prefab = Instantiate(Resources.Load("TextPrefab")) as GameObject;
                        prefab = PreparePrefab(prefab, tag, areaId);

                        // Link to next page button
                        prefab.GetComponent<ChangePage>().SetObj(prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        
                        prefab.GetComponent<ReadText>().SetTextObject(prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        prefab.GetComponent<ReadText>().SetFileName(Path.Combine(FileListHandler.GetPath(), f));
                    }

                    selectedFiles.text = selectedFiles.text.Replace($"\n-{f}", "");
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

    public int GetNearestAreaFromTag(string tag)
    {
        int id = -1;
        float minDist = float.MaxValue;
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

    public GameObject PreparePrefab(GameObject prefab, string tag, int areaId)
    {               
        int availableSlot = CheckAvailableSlot(prefab, tag);
        prefab.transform.position = GetPositionBasedOnTag(prefab, availableSlot, tag);
        prefab = RotateBasedOnTag(prefab, tag);
        prefab.GetComponent<PrefabData>().SetTagAreaId(areaId);
        prefab.GetComponent<PrefabData>().SetTagAreaSlotId(availableSlot);
        TagSceneHandler.GetTagAreaList()[areaId].SetSlotAvailability(availableSlot, false);

        // Link to close button
        prefab.GetComponent<Close>().SetObj(prefab);

        return prefab;
    }

    public Vector3 GetPositionBasedOnTag(GameObject prefab, int slot, string tag)
    {
        Vector3 position = new Vector3(0,0,0);
        int areaId = GetNearestAreaFromTag(tag);
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

    public GameObject RotateBasedOnTag(GameObject prefab, string tag)
    {
        int areaId = GetNearestAreaFromTag(tag);

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

    public int CheckAvailableSlot(GameObject prefab, string tag)
    {
        int maxSlots = 4;
        int areaId = GetNearestAreaFromTag(tag);

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

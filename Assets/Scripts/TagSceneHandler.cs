using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using TMPro;
using System.Linq;

public class TagSceneHandler : MonoBehaviour
{
    public GameObject buttonMenu;

    private GameObject mainCamera;
    private static List<TagArea> tagAreaList = new List<TagArea>();
    private static List<GameObject> tempTagAreaList = new List<GameObject>();

    float dist = 1.0f;

    /**
    * Start is called before the first frame update
    * If tag areas were previously generated, they are loaded again
    **/
    public void Start()
    {
        mainCamera = GameObject.Find("Main Camera");

        // Show every previously generated tag area when reloading the tag scene

        tempTagAreaList.Clear();

        foreach (TagArea ta in tagAreaList)
        {
            GameObject tagAreaPrefab = Instantiate(Resources.Load("TagAreaPrefab")) as GameObject;
            tagAreaPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = ta.GetTag();

            tagAreaPrefab.transform.position = ta.GetPosition();
            tagAreaPrefab.transform.localScale = ta.GetScale();
            tagAreaPrefab.transform.localRotation = ta.GetRotation();

            tempTagAreaList.Add(tagAreaPrefab);
        }

        tagAreaList.Clear();

        // Creates buttons based on the retrieve tags from the json file

        FileTagList ftl = FileTagList.GetFileTagList();        
        List<string> trimmedList = ftl.tagList.Distinct().ToList();
        trimmedList.Add("[Untagged]");

        foreach (string tag in trimmedList)
        {
            GameObject button = Instantiate(Resources.Load("ButtonPrefab")) as GameObject;
            button.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate{GenerateTagArea(tag);});
            button.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = tag;
            button.transform.SetParent(buttonMenu.transform, false);
        }
    }

    /**
    * Change the current scene to the main menu scene
    **/
    public void SwitchToMenuScene()
    {
        foreach (GameObject go in tempTagAreaList)
        {
            string tag = go.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text;
            tagAreaList.Add(new TagArea(tag, go.transform.position, go.transform.localScale, go.transform.localRotation));
        }

        SceneManager.LoadScene("MainScene");
    }

    /**
    * Remove all placed tag areas
    **/
    public void RemoveAll()
    {
        tempTagAreaList.Clear();

        GameObject[] tagAreaPrefabs = GameObject.FindGameObjectsWithTag("TagAreaPrefab");
        
        foreach (GameObject tap in tagAreaPrefabs) 
        {
            Destroy(tap);
        }
    }

    /**
    * Create a tag area prefab in the scene
    *
    * @param tag : the associated tag of the tag area 
    **/
    public void GenerateTagArea(string tag)
    {
        GameObject tagAreaPrefab = Instantiate(Resources.Load("TagAreaPrefab")) as GameObject;

        tagAreaPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = tag;
        tagAreaPrefab.transform.position = mainCamera.transform.position + mainCamera.transform.forward * dist;
        tagAreaPrefab.transform.rotation = Quaternion.Euler(0, mainCamera.transform.eulerAngles.y, 0);

        tempTagAreaList.Add(tagAreaPrefab);
    }

    /**
    * Return the list of placed tag areas in the scene
    *
    * @return a list of tag areas
    **/
    static public List<TagArea> GetTagAreaList()
    {
        return tagAreaList;
    }

    /**
    * Return the list of temporary placed tag areas in the scene
    *
    * @return a list of gameobject of tag areas
    **/
    public List<GameObject> GetTempTagAreaList()
    {
        return tempTagAreaList;
    }
}

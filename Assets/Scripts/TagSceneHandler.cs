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

    private GameObject camera;
    private static List<TagArea> tagAreaList = new List<TagArea>();
    private static List<GameObject> tempTagAreaList = new List<GameObject>();

    /**
    * Start is called before the first frame update
    * If tag areas were previously generated, they are loaded again
    **/
    void Start()
    {
        camera = GameObject.Find("Main Camera");

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

        StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/FileTagList.json");
        FileTagList ftl = JsonUtility.FromJson<FileTagList>(reader.ReadToEnd());
        
        List<string> trimmedList = ftl.tagList.Distinct().ToList();
        trimmedList.Add("[Untagged]");

        foreach (string tag in trimmedList)
        {
            GameObject newButton = Instantiate(Resources.Load("ButtonPrefab")) as GameObject;
            newButton.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate{GenerateTagArea(tag);});
            newButton.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = tag;
            newButton.transform.SetParent(buttonMenu.transform, false);
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
    * Create a tag area prefab in the scene
    *
    * @param tag : the associated tag of the tag area 
    **/
    public void GenerateTagArea(string tag)
    {
        float dist = 1.0f;
        GameObject tagAreaPrefab = Instantiate(Resources.Load("TagAreaPrefab")) as GameObject;

        tagAreaPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = tag;
        tagAreaPrefab.transform.position = camera.transform.position + camera.transform.forward * dist;
        tagAreaPrefab.transform.rotation = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0);

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
}

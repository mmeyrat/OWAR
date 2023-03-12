using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TagSceneHandler : MonoBehaviour
{
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
            tagAreaPrefab.tag = ta.GetTag();

            tagAreaPrefab.transform.position = ta.GetPosition();
            tagAreaPrefab.transform.localScale = ta.GetScale();
            tagAreaPrefab.transform.localRotation = ta.GetRotation();

            tempTagAreaList.Add(tagAreaPrefab);
        }

        tagAreaList.Clear();
    }

    /**
    * Change the current scene to the main menu scene
    **/
    public void SwitchToMenuScene()
    {
        foreach (GameObject go in tempTagAreaList)
        {
            tagAreaList.Add(new TagArea(go.tag, go.transform.position, go.transform.localScale, go.transform.localRotation));
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
        int dist = 1;
        GameObject tagAreaPrefab = Instantiate(Resources.Load("TagAreaPrefab")) as GameObject;

        tagAreaPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = tag;
        tagAreaPrefab.transform.position = camera.transform.position + camera.transform.forward * dist;
        tagAreaPrefab.transform.rotation = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0);
        tagAreaPrefab.tag = tag;

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

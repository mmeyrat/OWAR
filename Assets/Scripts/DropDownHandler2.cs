using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI.CoroutineTween;
using System;

public class DropDownHandler2 : MonoBehaviour
{
    // To deploy on HoloLens use this path (works also on Unity):
    private static string path = Application.streamingAssetsPath;
    private static Dictionary<string, bool> choosenFiles = new Dictionary<string, bool>();
    private static string[] fileEntries;
    private static bool alreadyDisplayed = false;

    public GameObject ItemTemplate;

    /**
    * Start is called before the first frame update
    **/
    void Start()
    {
        // Display all files names available in the dropdown list
        fileEntries = Directory.GetFiles(path);

        foreach (string file in fileEntries)
        {
            if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".txt"))
            {
                // Add file in the UI menu list
                AddItemToList(file.Substring(path.Length + 1));

                if (alreadyDisplayed == false)
                {
                    choosenFiles.Add(file.Substring(path.Length + 1), false);
                }
            }
        }

    }

    /// <summary>
    /// Add a file to the UI menu list
    /// </summary>
    /// <param name="file">added file's name</param>
    private void AddItemToList(string file)
    {
        // Duplicate item template and add it to the list (and set parent)
        GameObject NewListItem = Instantiate(ItemTemplate, this.transform.GetChild(0));

        // Set text of instantiated item
        Text NewListItemText = NewListItem.GetComponentInChildren<Text>();
        NewListItemText.text = file; // TODO maybe clean by not use a variable

        // Set active the new item 
        NewListItem.SetActive(true);

        // Set event to toggle on the item
        var ItemToggle = ItemTemplate.transform.GetChild(2);
        Toggle ItemToggleComponent = (Toggle)ItemToggle.GetComponent<Toggle>();     

        // Toggle listener on value changed is set in unity inspector (because it works better)
       
    }

    /**
   * Return if a file has been already choosen 
   * 
   * @param file : the selected file     
   * @return true or false
   **/
    public static bool IsFileChoosen(string file)
    {
        return choosenFiles[file];
    }

    /**
    * Unselect every file to display the choosen ones
    **/
    public static void HasBeenDisplayed()
    {
        alreadyDisplayed = true;

        foreach (string file in fileEntries)
        {
            choosenFiles[file] = false;
        }
    }

    /**
    * Select a file to display
    * 
    * @param file : the file to select
    **/
    public static void SetToChoosen(string file)
    {        
        choosenFiles[file] = !choosenFiles[file];     
    }

    /**
    * Return the path of the directory where the files are located
    * 
    * @return directory path
    **/
    public static string GetPath()
    {
        return path;
    }

    /**
    * Return the files available to display
    * 
    * @return an array of files
    **/
    public static string[] GetFiles()
    {
        fileEntries = Directory.GetFiles(path);

        for (int i = 0; i < fileEntries.Length; i++)
        {
            fileEntries[i] = fileEntries[i].Substring(path.Length + 1);
        }

        return fileEntries;
    }

}

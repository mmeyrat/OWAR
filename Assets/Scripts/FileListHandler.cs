using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI.CoroutineTween;
using System;

public class FileListHandler : MonoBehaviour
{   
    // To deploy on HoloLens use this path (works also on Unity):
    private static string path = Application.streamingAssetsPath;
    private static Dictionary<string, bool> choosenFiles = new Dictionary<string, bool>(); 
    private static string[] fileEntries;
    private static bool alreadyDisplayed = false;

    /**
    * Start is called before the first frame update
    **/
    void Start()
    {
        // Display all files names available in the dropdown list
        fileEntries = Directory.GetFiles(path);

        if (choosenFiles.Count > 0)
        {
            choosenFiles.Clear();
        }
        
        foreach (string file in fileEntries) 
        {
            if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".txt")) 
            {
                // Add file in the UI menu list
                AddItemToList(file.Substring(path.Length + 1));
                
                if (!alreadyDisplayed) 
                {
                    choosenFiles.Add(file.Substring(path.Length + 1), false);
                }
            }
        }

        // Get the number of options
        //print(dropdown.options.Count);
    }

    /**
    * Add a file to the UI menu list
    *
    * @param : added file's name
    **/
    private void AddItemToList(string file)
    {StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/FileTagList.json");
            FileTagList ftl = JsonUtility.FromJson<FileTagList>(reader.ReadToEnd());

            string tag = "[Untagged]";
                
                if (ftl.fileList.Contains(file))
                {
                    tag = ftl.tagList[ftl.fileList.IndexOf(file)];
                }
        // Duplicate item template and add it to the list (and set parent)
        GameObject listItem = Instantiate(Resources.Load("ListItemPrefab"), this.transform.GetChild(0)) as GameObject;

        // Set text of instantiated item
        Text label = listItem.transform.GetChild(1).GetChild(1).GetComponent<Text>();
        label.text = file;

        listItem.transform.GetChild(1).GetChild(2).GetComponent<Text>().text = tag;

        // Set event to toggle on the item
        MainSceneHandler msh = GameObject.Find("Menu").GetComponent<MainSceneHandler>();
        Toggle toggle = listItem.transform.GetChild(1).GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate{msh.FileSelector(label);});
    }

    /**
    * Return if a file has been already choosen 
    * 
    * @param file : the selected file     
    * @return true or false
    **/
    public static bool IsFileChoosen(string file) 
    {
        if (choosenFiles.ContainsKey(file))
        {
            return choosenFiles[file];
        }

        return false;
    } 

    /**
    * Unselect every file to display the choosen ones
    **/
    public static void HasBeenDisplayed() 
    {
        alreadyDisplayed = true;

        foreach(string file in fileEntries) 
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

    /**
    * Return the number of selected files
    * 
    * @return a quantity of files
    **/
    public static int GetNumberOfChoosenFiles()
    {
        int nbChoosenFiles = 0;

        foreach (string key in choosenFiles.Keys)
        {
            if (choosenFiles[key])
            {
                nbChoosenFiles++;
            }
        }

        return nbChoosenFiles;
    }
}

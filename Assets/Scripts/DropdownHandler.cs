using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class DropdownHandler : MonoBehaviour
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
        Dropdown dropdown = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();

        // Display all files names available in the dropdown list
        fileEntries = Directory.GetFiles(path);
        
        foreach (string file in fileEntries) 
        {
            if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".txt")) 
            {
                dropdown.options.Add(new Dropdown.OptionData(){ text = file.Substring(path.Length + 1) });
                
                if (alreadyDisplayed == false) 
                {
                    choosenFiles.Add(file.Substring(path.Length + 1), false);
                }
            }
        }
        // Get the number of options
        //print(dropdown.options.Count);
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

<<<<<<< HEAD
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
        for (int i = 0; i < fileEntries.Length; i++) 
        {
            fileEntries[i] = fileEntries[i].Substring(path.Length + 1);
=======
    public static string[] files() {
        fileEntries = Directory.GetFiles(path);
        for (int i=0; i<fileEntries.Length; i++) {
            fileEntries[i] = fileEntries[i].Substring(path.Length+1);
>>>>>>> 8c8b329 (Fixing multiview and menu on same scene + close button)
        }

        return fileEntries;
    }
}

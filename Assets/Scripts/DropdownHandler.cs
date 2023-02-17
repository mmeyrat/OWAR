using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DropdownHandler : MonoBehaviour
{   
    // To deploy on Holo use this path (works also on Unity):
    private static string path = Application.streamingAssetsPath;

    private static Dictionary<string, bool> choosenFiles = new Dictionary<string, bool>(); 
    private static string[] fileEntries;
    private static bool alreadyDisplayed = false;

    // Start is called before the first frame update
    void Start()
    {
        var dropdown = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();

        // Display all files names available in the dropdown list
        fileEntries = Directory.GetFiles(path);
        foreach(string file in fileEntries) {
            if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".txt")) {
                dropdown.options.Add(new Dropdown.OptionData(){ text = file.Substring(path.Length+1) });
                if (alreadyDisplayed == false) {
                    choosenFiles.Add(file.Substring(path.Length+1), false);
                }
            }
        }
    }

    public static string getPath() {
        return path;
    }

    public static bool isFileChoosen(string file) {
        return choosenFiles[file];
    } 

    public static void setToChoosen(string file) {
        choosenFiles[file] = !choosenFiles[file];
    }

    public static string[] files() {
        for (int i=0; i<fileEntries.Length; i++) {
            fileEntries[i] = fileEntries[i].Substring(path.Length+1);
        }

        return fileEntries;
    }

    public static void hasBeenDisplayed() {
        alreadyDisplayed = true;
        foreach(string file in fileEntries) {
            choosenFiles[file] = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class DropdownToVisual : MonoBehaviour
{
    public Dropdown dropdownList;
    public Button button;
    public Text selectedFiles;

    private int maxLineLength = 17;

    /**
    * Add the selecetd files in a text preview
    **/
    public void FileSelector() 
    {
        int index = dropdownList.value;
        string file = dropdownList.options[index].text;

        DropdownHandler.SetToChoosen(file);

        if (DropdownHandler.IsFileChoosen(file)) 
        {
            selectedFiles.text += $"\n - {file}";
        } 
        else 
        {
            selectedFiles.text = selectedFiles.text.Replace($"\n - {file}", "");
        }
    }

    /**
    * Change the scene to display the files
    **/
    public void Visualize() 
    {
        if (selectedFiles.text.Length > maxLineLength) 
        {
            SceneManager.LoadScene("Visualizer");
        } 
        else 
        {
            // TODO : Popup to show alert no files selected
        }
    }
}

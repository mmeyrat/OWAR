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

    public void fileSelector() {
        int index = dropdownList.value;
        string file = dropdownList.options[index].text;
        DropdownHandler.setToChoosen(file);
        if (DropdownHandler.isFileChoosen(file) == true) {
            selectedFiles.text += "\n" + " - " + file;
        } else {
            selectedFiles.text = selectedFiles.text.Replace("\n" + " - " + file, "");
        }
    }

    public void visualize() {
        if (selectedFiles.text.Length > 17) {
            SceneManager.LoadScene("Visualizer");
        } else {
            // TODO : Popup to show alert no files selected
        }
    }
}

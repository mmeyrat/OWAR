using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using TMPro;

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
            float offsetImage = 0.2f;
            float offsetImageIncrement = 3.0f;
            float offsetText = - 0.2f;
            float offsetTextIncrement = 2.1f;
            string[] files = DropdownHandler.GetFiles();
            
            foreach (string f in files) 
            {
                if (f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png")) 
                {
                    if (DropdownHandler.IsFileChoosen(f)) 
                    {
                        GameObject imagePoster = Instantiate(Resources.Load("ImagePoster")) as GameObject;
                        imagePoster.GetComponent<Close>().SetObj(imagePoster);
                        imagePoster.GetComponent<DisplayImage>().SetPoseX(offsetImage);
                        imagePoster.GetComponent<DisplayImage>().SetFilename(Path.Combine(DropdownHandler.GetPath(), f));

                        offsetImage *= offsetImageIncrement;

                        selectedFiles.text = selectedFiles.text.Replace($"\n - {f}", "");
                        DropdownHandler.SetToChoosen(f);
                    } 
                }
                else if (f.EndsWith(".txt")) 
                {   
                    if (DropdownHandler.IsFileChoosen(f)) 
                    {
                        GameObject text3D = Instantiate(Resources.Load("Canvas")) as GameObject;
                        text3D.transform.localPosition = new Vector3(offsetText, text3D.transform.localPosition.y, text3D.transform.localPosition.z);
                        text3D.GetComponent<Close>().SetObj(text3D);
                        text3D.GetComponent<ChangePage>().SetObj(text3D.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        text3D.GetComponent<ReadText>().SetFilename(Path.Combine(DropdownHandler.GetPath(), f));
                        text3D.GetComponent<ReadText>().SetTextMesh(text3D.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        text3D.GetComponent<ReadText>().SetCollider(text3D.transform.GetChild(0).GetChild(0).GetComponent<BoxCollider>());

                        offsetText *= offsetTextIncrement;

                        selectedFiles.text = selectedFiles.text.Replace($"\n - {f}", "");
                        DropdownHandler.SetToChoosen(f);
                    }
                }
            }
        } else {
            // TODO : Popup to show alert no files selected
        }
    }
}

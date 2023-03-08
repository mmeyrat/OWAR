using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using TMPro;

public class DropDownToVisual2 : MonoBehaviour
{
    public GameObject FileList;
    public Button button;
    public Text selectedFiles;

    private int maxLineLength = 17;

    /**
    * Add the selecetd files in a text preview
    **/
    public void FileSelector(Text label)
    {
        string file = label.text;

        DropDownHandler2.SetToChoosen(file);

        if (DropDownHandler2.IsFileChoosen(file))
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
            float offsetText = -0.2f;
            float offsetTextIncrement = 2.1f;
            string[] files = DropDownHandler2.GetFiles();

            foreach (string f in files)
            {
                if (f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png"))
                {
                    if (DropDownHandler2.IsFileChoosen(f))
                    {
                        GameObject imagePrefab = Instantiate(Resources.Load("ImagePrefab")) as GameObject;
                        imagePrefab.transform.localPosition = new Vector3(offsetText, imagePrefab.transform.localPosition.y, imagePrefab.transform.localPosition.z);

                        // Link to close button
                        imagePrefab.GetComponent<Close>().SetObj(imagePrefab);

                        imagePrefab.GetComponent<DisplayImage>().SetImageObject(imagePrefab.transform.GetChild(0).GetChild(0).GetComponent<RawImage>());
                        imagePrefab.GetComponent<DisplayImage>().SetFileName(Path.Combine(DropDownHandler2.GetPath(), f));
                        imagePrefab.GetComponent<DisplayImage>().SetPoseX(offsetImage);

                        offsetImage *= offsetImageIncrement;

                        // Remove file from UI selected files list
                        selectedFiles.text = selectedFiles.text.Replace($"\n - {f}", "");
                        //DropDownHandler3.SetToChoosen(f); // TODO FIX : check when it's called because it's not necessary here
                    }
                }
                else if (f.EndsWith(".txt"))
                {
                    if (DropDownHandler2.IsFileChoosen(f))
                    {
                        GameObject textPrefab = Instantiate(Resources.Load("TextPrefab")) as GameObject;
                        textPrefab.transform.localPosition = new Vector3(offsetText, textPrefab.transform.localPosition.y, textPrefab.transform.localPosition.z);

                        // Link to close button
                        textPrefab.GetComponent<Close>().SetObj(textPrefab);
                        // Link to next page button
                        textPrefab.GetComponent<ChangePage>().SetObj(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());

                        textPrefab.GetComponent<ReadText>().SetTextObject(textPrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
                        textPrefab.GetComponent<ReadText>().SetFileName(Path.Combine(DropDownHandler2.GetPath(), f));

                        offsetText *= offsetTextIncrement;

                        // Remove file from UI selected files list
                        selectedFiles.text = selectedFiles.text.Replace($"\n - {f}", "");
                        //DropDownHandler3.SetToChoosen(f); // TODO FIX : check when it's called because it's not necessary here
                    }
                }
            }
        }
        else
        {
            // TODO : Popup to show alert no files selected
        }

        // Untoggle visualized file in the list
        GameObject PanelListComponent = FileList.transform.GetChild(0).gameObject;

        for (int i = 1; i < PanelListComponent.transform.childCount; i++) // Start at 1 to avoid the disabled template item
        {
            GameObject currentChild = PanelListComponent.transform.GetChild(i).gameObject;

            //Check if it's an item which corresponds to a selected file 
            Text currentChildText = currentChild.transform.GetChild(1).GetComponent<Text>();
            string filename = currentChildText.text;

            if (DropDownHandler2.IsFileChoosen(filename))
            {     
                Toggle currentChildToggle = (Toggle)currentChild.transform.Find("Toggle").GetComponent<Toggle>();
                currentChildToggle.isOn = false;

            }
            
            
        }
        
    }
}

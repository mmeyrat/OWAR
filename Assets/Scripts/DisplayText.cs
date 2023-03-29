using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Text;
using TMPro;

public class DisplayText : MonoBehaviour
{
    private TMP_Text textObject;
    private GameObject mainCamera;
    private float minFontSize;
    private string fileName;

    private int maxDecimal = 2; // For LOD gap between size changes
    private float maxFontSize = 0.1f; // For LOD changes
 
    /**
    * Start is called before the first frame update
    **/
    void Start()
    {
        LoadText();

        minFontSize = textObject.fontSize;
        mainCamera = GameObject.Find("Main Camera");
    }

    /**
    * Update is called at each frame update
    **/
    void Update()
    {
        if (mainCamera != null)
        {
            float dist = Vector3.Distance(textObject.transform.position, mainCamera.transform.position);
            // Round the size to avoid unpleasant small changes
            float roundedFontSize = (float) Math.Round((double) (minFontSize * dist), maxDecimal);
            
            textObject.fontSize = Mathf.Min(Mathf.Max(roundedFontSize, minFontSize), maxFontSize);

            if (textObject.pageToDisplay > textObject.textInfo.pageCount)
            {
                textObject.pageToDisplay = textObject.textInfo.pageCount;
            }
        }
    }

    /**
    * Load the text from the file path
    **/
    public void LoadText()
    {
        if (File.Exists(fileName))
        {
            string textContent = File.ReadAllText(fileName);
            textObject.text = textContent;
        }
        else 
        {
            textObject.text = "Loading error.";
        }
    }

    /**
    * Set the file path
    * 
    * @param name : path of the file 
    **/
    public void SetFileName(string name) 
    {
        this.fileName = name; 
    }

    /**
    * Set the text object
    * 
    * @param obj : the text object 
    **/
    public void SetTextObject(TMP_Text obj) 
    {
        this.textObject = obj;
    }

    /**
    * Return the value of the file path
    *
    * @return file path
    **/
    public string GetFileName() 
    {
        return this.fileName;
    }
    
    /**
    * Return the value of the text object
    * 
    * @return text object 
    **/
    public TMP_Text GetTextObject() 
    {
        return this.textObject;
    }
}
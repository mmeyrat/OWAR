using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Text;
using TMPro;

public class ReadText : MonoBehaviour
{
    public float minFontSize = 0.03f;
    public float maxFontSize = 0.1f;

    private TMP_Text textObject;
    private GameObject camera;
    private string fileName;

    /**
    * Start is called before the first frame update
    **/
    void Start()
    {
        // TO USE LATER Path.GetFullPath("test2.txt");
        string textContent = File.ReadAllText(fileName);

        textObject.text = textContent;
        camera = GameObject.Find("Main Camera");
    }

    /**
    * Update is called at each frame update
    **/
    void Update()
    {
        int maxDecimal = 3;
        float dist = Vector3.Distance(textObject.transform.position, camera.transform.position);
        // Round the size to avoid unpleasant small changes
        float roundedFontSize = (float) Math.Round((double) (minFontSize * dist), maxDecimal);
        
        textObject.fontSize = Mathf.Min(Mathf.Max(roundedFontSize, minFontSize), maxFontSize);
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
}
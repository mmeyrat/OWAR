using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Text;

public class ReadText : MonoBehaviour
{
    public GameObject background;
    
    private BoxCollider boxCollider;
    private TextMesh textMesh;
    private string fileName;
    private int maxLineLength = 20;

    /**
    * Start is called before the first frame update
    **/
    void Start()
    {
        // TO USE LATER Path.GetFullPath("test2.txt");
        string textContent = File.ReadAllText(fileName);
        // Format the text with a maximum length for each line of 15 characters
        string formatedText = FormatText(textContent);
        // Get the total number of lines of the text
        int height = formatedText.Split('\n').Length - 1;

        // set text and resize background and collider
        textMesh.text = formatedText;
        ResizeWindow(height);
    }

    /**
    * Format the text, each line got the same length
    *
    * @param text : text to format
    * @param lineLength : length of each line of the text
    * @return text in the wanted format
    **/
    public string FormatText(string text) 
    {
        var builder = new StringBuilder();

        for (int i = 0; i < text.Length; i++) 
        {
            builder.Append(text[i]);
            
            if (i != 0 && i % maxLineLength == 0) 
            {
                builder.Append(Environment.NewLine);
            }
        }

        return builder.ToString();
    }

    /**
    * Method to resize the background and the collider containing the text to display
    * 
    * @param height : height of the window 
    **/
    void ResizeWindow(int height) {
        float heightWeightingValue = 2.0f;
        float widthWeightingValue = 10.0f;
        float sizeHeightOffset = 1.0f;
        float depthValue = 0.001f;

        Vector3 newSize = new Vector3(maxLineLength - (maxLineLength / widthWeightingValue), heightWeightingValue * height + sizeHeightOffset, depthValue); 

        this.boxCollider.size = newSize;
        this.background.transform.localScale = newSize; 
    }

    /**
    * Set the file path
    * 
    * @param name : path of the file 
    **/
    public void SetFilename(string name) {
        this.fileName = name; 
    }

    /**
    * Set the text mesh
    * 
    * @param mesh : the text mesh 
    **/
    public void SetTextMesh(TextMesh mesh) {
        this.textMesh = mesh;
    }

    /**
    * Set the box collider
    * 
    * @param collider : the box collider 
    **/
    public void SetCollider(BoxCollider collider) {
        this.boxCollider = collider;
    }
}
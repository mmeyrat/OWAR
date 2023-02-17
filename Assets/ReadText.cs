using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Text;

public class ReadText : MonoBehaviour
{
    private BoxCollider collider;
    public GameObject background;
    private TextMesh textMesh;
    private string fileName;
    private int maxLineLength = 20;

    // Start is called before the first frame update
    void Start()
    {
        //textMesh = GameObject.Find("3DText").GetComponent<TextMesh>();
        // string textContent = textFile.text;
        // TO USE LATER Path.GetFullPath("test2.txt");
        string textContent = File.ReadAllText(fileName);
        int width = 20; 

        // Format the text with a maximum length for each line of 15 characters
        string formatedText = formatText(textContent);
        // Get the total number of lines of the text
        int height = formatedText.Split('\n').Length - 1;


        // set text and resize background and collider
        textMesh.text = formatedText;
        resizeWindow(height);
    }

    /**
    * Format the text, each line got the same length
    * @param text : text to format
    * @param lineLength : length of each line of the text
    * @return text in the wanted format
    **/
    string formatText(string text) {
        var builder = new StringBuilder();
        for (int i = 0; i < text.Length; i++) {
            builder.Append(text[i]);
            if (i != 0 && i % maxLineLength == 0) {
                builder.Append(Environment.NewLine);
            }
        }
        return builder.ToString();
    }

    /**
    * Method to resize the background and the collider containing the text to display
    * @param height : height of the window 
    **/
    void resizeWindow(int height) {
        float sizeWidthOffset = 2.0f;
        float sizeHeightOffset = - 35.0f;
        float centerWidthOffset = 3.0f;
        float centerHeightOffset = 20.0f;

        collider.center = new Vector3(maxLineLength / centerWidthOffset, - height + centerHeightOffset, 0);
        collider.size = new Vector3(maxLineLength - sizeWidthOffset, 2 * height + sizeHeightOffset, 1.0f);

        background.transform.localPosition = new Vector3(maxLineLength / centerWidthOffset, - height + centerHeightOffset, 0.5f);
        background.transform.localScale = new Vector3(maxLineLength - sizeWidthOffset, 2 * height + sizeHeightOffset, 0.001f);
    }


    public void setFilename(string name) {
        fileName = name; 
    }

    public void setTextMesh(TextMesh mesh) {
        textMesh = mesh;
    }

    public void setCollider(BoxCollider collider) {
        this.collider = collider;
    }
}
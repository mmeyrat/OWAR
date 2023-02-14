using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Text;

public class ReadText : MonoBehaviour
{
    public TextAsset textFile;
    public BoxCollider collider;
    public GameObject background;
    private TextMesh textMesh;

    // Start is called before the first frame update
    void Start()
    {
        textMesh = GameObject.Find("3DText").GetComponent<TextMesh>();
        string textContent = textFile.text;
        int width = 20; 

        // format text with a length for each line of 15 characters
        string formatedText = formatText(textContent, width);
        int height = getTextHeight(formatedText);

        // set text and resize background and collider
        textMesh.text = formatedText;
        resizeWindow(width, height);
    }

    /**
    * Format the text, each line got the same length
    * @param text : text to format
    * @param lineLength : length of each line of the text
    * @return text in the wanted format
    **/
    string formatText(string text, int lineLength) {
        var builder = new StringBuilder();
        for (int i=0; i<text.Length; i++) {
            builder.Append(text[i]);
            if (i!= 0 && i % lineLength == 0) {
                builder.Append(Environment.NewLine);
            }
        }

        return builder.ToString();
    }

    /**
    * Get the number of lines of the text
    * @param text : text to display in string format 
    * @return the number of line of this text
    **/
    int getTextHeight(string text) {
        int height = 0;
        for (int i=0; i<text.Length; i++) {
            if (text[i] == '\n') {
                height++;
            }
        }

        return height;
    }

    /**
    * Method to resize the background and the collider containing the text to display
    * @param width : width of the window
    * @param height : height of the window 
    **/
    void resizeWindow(int width, int height) {
        print(height);
        collider.center = new Vector3(width/3.0f, -height+11.0f, 0);
        background.transform.localPosition = new Vector3(width/3.0f, -height+11.0f, 0.5f);
        background.transform.localScale = new Vector3(width - 2.0f, 2*height-20.0f, 0.001f);
        collider.size = new Vector3(width - 2.0f, 2*height-20.0f, 1);
    }
}
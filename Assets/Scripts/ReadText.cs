using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ReadText : MonoBehaviour
{
    public GameObject textArea;
    public int maxTextSize;

    // Start is called before the first frame update
    void Start()
    {
        string filePath = "Assets/Texts/test.txt";
        string textContent = File.ReadAllText(filePath);

        if (textContent.Length > maxTextSize) {
            textContent = textContent.Substring(0, maxTextSize) + "...";
        }

        textArea.GetComponent<Text>().text = textContent;
    }
}

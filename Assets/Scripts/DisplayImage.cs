using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Drawing;

public class DisplayImage : MonoBehaviour
{
    public GameObject imagePoster;
    private string fileName;

    // Start is called before the first frame update
    void Start()
    {
        //imagePoster = GameObject.Find("ImagePoster");
        Texture2D image = new Texture2D(1, 1);
		image.LoadImage(File.ReadAllBytes(fileName));
        imagePoster.GetComponent<Renderer>().material.mainTexture = image;
    }

    public void setFilename(string name) {
        fileName = name;
    }
}

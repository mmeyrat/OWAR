using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Drawing;

public class DisplayImage : MonoBehaviour
{
    private GameObject imagePoster;
    public Texture texture;

    // Start is called before the first frame update
    void Start()
    {
        imagePoster = GameObject.Find("ImagePoster");
        imagePoster.GetComponent<Renderer>().material.mainTexture = texture;
    }
}

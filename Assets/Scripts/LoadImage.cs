using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadImage : MonoBehaviour
{
    public GameObject ImageArea;

    // Start is called before the first frame update
    void Start()
    {
        Texture2D image = new Texture2D(1, 1);
		image.LoadImage(File.ReadAllBytes("Assets/Images/giorno.png"));
        ImageArea.GetComponent<RawImage>().texture = image;
    }
}

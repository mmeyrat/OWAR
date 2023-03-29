using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Drawing;
using System;

public class DisplayImage : MonoBehaviour
{
    private RawImage imageObject;
    private GameObject mainCamera;
    private float minSize;
    private string fileName;

    private int textureSize = 1;
    private int maxDecimal = 2; // For LOD gap between size changes
    private float maxDist = 2.0f; // For LOD changes

    /**
    * Start is called before the first frame update
    * The image is loaded and applied to the object
    **/
    void Start()
    {
        LoadImage();

        minSize = imageObject.GetComponent<RectTransform>().sizeDelta.x;
        mainCamera = GameObject.Find("Main Camera");
    }

    /**
    * Update is called at each frame update
    **/
    public void Update()
    {
        float dist = Vector3.Distance(imageObject.transform.position, mainCamera.transform.position);
        // Round the size to avoid unpleasant small changes
        float roundedSize = (float) Math.Round((double) (dist * minSize), maxDecimal);
        float newSize = Mathf.Min(Mathf.Max(roundedSize, minSize), minSize * maxDist);

        imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (newSize, newSize);
    }

    /**
    * Load the image from the file path
    **/
    public void LoadImage()
    {
        if (File.Exists(fileName))
        {
            Texture2D texture = new Texture2D(textureSize, textureSize);
            texture.LoadImage(File.ReadAllBytes(fileName));
            imageObject.texture = texture;
        }
    }

    /**
    * Set the image object
    * 
    * @param obj : the image object 
    **/
    public void SetImageObject(RawImage obj) 
    {
        this.imageObject = obj;
    }

    /**
    * Set the file path
    * 
    * @param name : the file path 
    **/
    public void SetFileName(string name) 
    {
        this.fileName = name;
    }

    /**
    * Return the value of the image object
    * 
    * @return image object 
    **/
    public RawImage GetImageObject() 
    {
        return this.imageObject;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Drawing;

public class DisplayImage : MonoBehaviour
{
    private RawImage imageObject;
    private GameObject camera;
    private string fileName;
    private float poseX = 0.0f;
    private float minSize;

    /**
    * Start is called before the first frame update
    **/
    public void Start()
    {
        Texture2D texture = new Texture2D(1, 1);
		texture.LoadImage(File.ReadAllBytes(fileName));

        imageObject.texture = texture;
        minSize = imageObject.GetComponent<RectTransform>().sizeDelta.x;
        camera = GameObject.Find("Main Camera");
    }

    /**
    * Update is called at each frame update
    **/
    public void Update()
    {
        float distOffset = 2.0f; 
        float dist = Vector3.Distance(imageObject.transform.position, camera.transform.position);
        float newSize = Mathf.Min(Mathf.Max(dist * minSize, minSize), minSize * distOffset);

        imageObject.GetComponent<RectTransform>().sizeDelta = new Vector2 (newSize, newSize);
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
    * Change the x position based on the offset
    * 
    * @param offset : the offset 
    **/
    public void SetPoseX(float offset) 
    {
        this.poseX += offset;
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

    /**
    * Return the value of the x position
    * 
    * @return x position
    **/
    public float GetPoseX() 
    {
        return this.poseX;
    }
}

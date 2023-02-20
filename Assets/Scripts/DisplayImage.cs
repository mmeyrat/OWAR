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
    private float pose_x = 0.0f;

    /**
    * Start is called before the first frame update
    **/
    public void Start()
    {
        Texture2D image = new Texture2D(1, 1);
		image.LoadImage(File.ReadAllBytes(fileName));

        imagePoster.GetComponent<Renderer>().material.mainTexture = image;
        imagePoster.transform.localPosition = new Vector3(pose_x, imagePoster.transform.localPosition.y , imagePoster.transform.localPosition.z);
    }

    /**
    * Set the file path
    * 
    * @param name : the file path 
    **/
    public void SetFilename(string name) 
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
        this.pose_x += offset;
    }

    /**
    * Return the value of the file path
    *
    * @return file path
    **/
    public string GetFilename() 
    {
        return fileName;
    }

    /**
    * Return the value of the x position
    * 
    * @return x position
    **/
    public float GetPoseX() 
    {
        return pose_x;
    }
}

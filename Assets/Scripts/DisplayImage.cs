using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Drawing;

public class DisplayImage : MonoBehaviour
{
    public GameObject imagePoster;

    private RawImage image;
    private string fileName;
    private float pose_x = 0.0f;
    private float baseSize;

    /**
    * Start is called before the first frame update
    **/
    public void Start()
    {
        Texture2D texture = new Texture2D(1, 1);
		texture.LoadImage(File.ReadAllBytes(fileName));

        image.texture = texture;
        baseSize = image.GetComponent<RectTransform>().sizeDelta.x;
        //imagePoster.GetComponent<Renderer>().material.mainTexture = image;
        //imagePoster.transform.localPosition = new Vector3(pose_x, imagePoster.transform.localPosition.y , imagePoster.transform.localPosition.z);
    }

    public void Update()
    {
        var camera = GameObject.Find("Main Camera");
        float dist = Vector3.Distance(image.transform.position, camera.transform.position);

        float newSize = Mathf.Min(Mathf.Max(dist * baseSize, baseSize), baseSize * 2.0f);

        image.GetComponent<RectTransform>().sizeDelta = new Vector2 (newSize, newSize);
    }

    /**
    * Set the file path
    * 
    * @param name : the file path 
    **/
    public void SetImage(RawImage image) 
    {
        this.image = image;
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

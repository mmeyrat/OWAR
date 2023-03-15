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
    private GameObject camera;
    private string fileName;
    private float poseX = 0.0f;
    private float minSize;
    private Vector3 velocity;
    private bool isCollided;
    private bool isCollidedWithCenter;

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
        InitVelocity();
    }

    /**
    * Update is called at each frame update
    **/
    public void Update()
    {
        int maxDecimal = 2;
        float maxDist = 2.0f; 
        float dist = Vector3.Distance(imageObject.transform.position, camera.transform.position);
        // Round the size to avoid unpleasant small changes
        float roundedSize = (float) Math.Round((double) (dist * minSize), maxDecimal);
        float newSize = Mathf.Min(Mathf.Max(roundedSize, minSize), minSize * maxDist);

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

    public void SetVelocity(Vector3 vel) {
        velocity += vel;
    }

    public void InitVelocity() {
        velocity = new Vector3(0, 0, 0);
    }

    public Vector3 GetVelocity() {
        return velocity;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.name == "CenterCollider") {
            isCollidedWithCenter = true;
        } else {
            isCollided = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        isCollided = false;
    }  

    public bool IsCollided() {
        return isCollided;
    }

    public bool IsCollidedWithCenter() {
        return isCollidedWithCenter;
    }
}

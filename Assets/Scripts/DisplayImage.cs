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

    // Start is called before the first frame update
    public void Start()
    {
        //imagePoster = GameObject.Find("ImagePoster");
        Texture2D image = new Texture2D(1, 1);
		image.LoadImage(File.ReadAllBytes(fileName));
        imagePoster.GetComponent<Renderer>().material.mainTexture = image;
        imagePoster.transform.localPosition = new Vector3(pose_x, imagePoster.transform.localPosition.y , imagePoster.transform.localPosition.z);
    }

    public void setFilename(string name) {
        fileName = name;
    }

    public void setPoseX(float offset) {
        pose_x += offset;
    }

    public string getFilename() {
        return fileName;
    }

    public float getPoseX() {
        return pose_x;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VisualizerLauncher : MonoBehaviour
{
    /**
    * Start is called before the first frame update
    **/
    void Start() 
    {
        float offsetImage = 0.2f;
        float offsetImageIncrement = 3.0f;
        float offsetText = - 0.2f;
        float offsetTextIncrement = 2.1f;
        string[] files = DropdownHandler.GetFiles();
        
        foreach (string f in files) 
        {
            if (f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png")) 
            {
                if (DropdownHandler.IsFileChoosen(f)) 
                {
                    GameObject imagePoster = Instantiate(Resources.Load("ImagePoster")) as GameObject;
                    imagePoster.GetComponent<DisplayImage>().SetPoseX(offsetImage);
                    imagePoster.GetComponent<DisplayImage>().SetFilename(Path.Combine(DropdownHandler.GetPath(), f));

                    offsetImage *= offsetImageIncrement;
                } 
            }
            else if (f.EndsWith(".txt")) 
            {   
                if (DropdownHandler.IsFileChoosen(f)) 
                {
                    GameObject text3D = Instantiate(Resources.Load("3DText")) as GameObject;
                    text3D.transform.localPosition = new Vector3(offsetText, text3D.transform.localPosition.y, text3D.transform.localPosition.z);
                    text3D.GetComponent<ReadText>().SetFilename(Path.Combine(DropdownHandler.GetPath(), f));
                    text3D.GetComponent<ReadText>().SetTextMesh(text3D.GetComponent<TextMesh>());
                    text3D.GetComponent<ReadText>().SetCollider(text3D.GetComponent<BoxCollider>());

                    offsetText *= offsetTextIncrement;
                }
            }
        }
    }
}

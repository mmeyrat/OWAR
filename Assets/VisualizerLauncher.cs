using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VisualizerLauncher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        float offset_image = 0.2f;
        float offset_text = -0.2f;
        string[] files = DropdownHandler.files();
        foreach(string f in files) {
            if (f.EndsWith(".jpg") || f.EndsWith(".jpeg") || f.EndsWith(".png")) {
                if (DropdownHandler.isFileChoosen(f)) {
                    GameObject imagePoster = Instantiate(Resources.Load("ImagePoster")) as GameObject;
                    imagePoster.GetComponent<DisplayImage>().setPoseX(offset_image);
                    imagePoster.GetComponent<DisplayImage>().setFilename(Path.Combine(DropdownHandler.getPath(), f));
                    offset_image *= 3;
                }
            } else if (f.EndsWith(".txt")) {
                if (DropdownHandler.isFileChoosen(f)) {
                    GameObject text3D = Instantiate(Resources.Load("3DText")) as GameObject;
                    text3D.transform.localPosition = new Vector3(offset_text, text3D.transform.localPosition.y, text3D.transform.localPosition.z);
                    text3D.GetComponent<ReadText>().setFilename(Path.Combine(DropdownHandler.getPath(), f));
                    text3D.GetComponent<ReadText>().setTextMesh(text3D.GetComponent<TextMesh>());
                    text3D.GetComponent<ReadText>().setCollider(text3D.GetComponent<BoxCollider>());
                    offset_text *= 2.1f;
                }
            }
        }
    }
}

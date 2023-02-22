using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Close : MonoBehaviour
{
    private GameObject objectToClose;

    public void setObj(GameObject obj) {
        objectToClose = obj;
    }

    public void CloseWindow() {
        Destroy(objectToClose);
    }
}

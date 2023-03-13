using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Close : MonoBehaviour
{
    private GameObject objectToClose;

    /**
    * Set the file to close
    **/
    public void SetObj(GameObject obj) 
    {
        objectToClose = obj;
    }

    /**
    * Close the current file
    **/
    public void CloseWindow() 
    {
        Destroy(objectToClose);
    }
}

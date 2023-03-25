using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Close : MonoBehaviour
{
    private GameObject objectToClose;
    private int indexZone;

    /**
    * Set the file to close
    **/
    public void SetObj(GameObject obj) 
    {
        objectToClose = obj;
    }

    /**
    * Set the index of the zone concerned
    **/
    public void SetIndexZone(int index) {
        indexZone = index;
    }

    /**
    * Close the current file
    **/
    public void CloseWindow() 
    {
        ApplyForces.RemoveObj(objectToClose, indexZone);
        Destroy(objectToClose);
    }

}

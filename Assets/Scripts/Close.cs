using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Close : MonoBehaviour
{
    private GameObject objectToClose;
    
    private int minTagAreaId = 0;

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
        if (objectToClose.GetComponent<PrefabData>())
        {
            int tagAreaId = objectToClose.GetComponent<PrefabData>().GetTagAreaId();
            int slotId = objectToClose.GetComponent<PrefabData>().GetTagAreaSlotId();

            if (tagAreaId >= minTagAreaId)
            {
                TagSceneHandler.GetTagAreaList()[tagAreaId].SetSlotAvailability(slotId, true);
            }
        }

        Destroy(objectToClose);
    }
}

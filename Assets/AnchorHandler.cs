using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class AnchorHandler : MonoBehaviour
{
    public GameObject anchoredObject;

    /** 
    * To switch between view anchor mode and world anchor mode 
    **/
    public void switchMode()
    {        
        // component that handles view anchor mode, so that handle follows movement transformations
        SolverHandler comp = anchoredObject.GetComponent<SolverHandler>(); 

        if (comp.enabled) // if object is view anchored
        {
            // Switch to world anchor mode
            comp.enabled = false;
        }
        else // if object is world anchored
        {
            // Switch to view anchor mode
            comp.enabled = true;

            PrefabData data = anchoredObject.GetComponent<PrefabData>();
            
            if (data) // Check if object is a displayed file
            {
                int tagAreaId = data.GetTagAreaId();
                int slotId = data.GetTagAreaSlotId();
                
                if (anchoredObject.transform.parent != null)
                {
                    GameObject parent = anchoredObject.transform.parent.gameObject;
                    anchoredObject.transform.SetParent(null);
                    Destroy(parent);
                }
                
                if (tagAreaId >= 0)
                {
                    TagSceneHandler.GetTagAreaList()[tagAreaId].SetSlotAvailability(slotId, true);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class AnchorHandler : MonoBehaviour
{
    public GameObject anchoredObject;

    /** 
    * To switch anchor mode of anchoredObject 
    **/
    public void switchMode()
    {        
        // component that handle view anchor mode, so handle follow movement transformation
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
            
            if (anchoredObject.GetComponent<PrefabData>()) // Check if object is a displayed file
            {
                int tagAreaId = anchoredObject.GetComponent<PrefabData>().GetTagAreaId();
                int slotId = anchoredObject.GetComponent<PrefabData>().GetTagAreaSlotId();
                
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

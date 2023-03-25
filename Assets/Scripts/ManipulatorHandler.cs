using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulatorHandler : MonoBehaviour
{   
    private int minTagAreaId = 0;

    /**
    * If the object is moved while in a slot of a tag area, it's removed from the slot of that tag area
    **/
    public void RemoveFromTagAreaSlot()
    {
        int tagAreaId = this.GetComponent<PrefabData>().GetTagAreaId();
        int slotId = this.GetComponent<PrefabData>().GetTagAreaSlotId();

        if (tagAreaId >= minTagAreaId)
        {
            if (!TagSceneHandler.GetTagAreaList()[tagAreaId].GetSlotAvailability(slotId))
            {
                if (this.transform.parent != null)
                {
                    GameObject parent = this.transform.parent.gameObject;
                    this.transform.SetParent(null);
                    Destroy(parent);
                }

                TagSceneHandler.GetTagAreaList()[tagAreaId].SetSlotAvailability(slotId, true);
            }
        }
    }
}

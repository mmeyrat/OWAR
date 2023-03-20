using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulatorHandler : MonoBehaviour
{   
    private int minTagAreaId = 0;

    public void RemoveFromTagAreaSlot()
    {
        int tagAreaId = this.GetComponent<PrefabData>().GetTagAreaId();
        int slotId = this.GetComponent<PrefabData>().GetTagAreaSlotId();

        if (tagAreaId >= minTagAreaId)
        {
            if (!TagSceneHandler.GetTagAreaList()[tagAreaId].GetSlotAvailability(slotId))
            {
                TagSceneHandler.GetTagAreaList()[tagAreaId].SetSlotAvailability(slotId, true);
            }
        }
    }
}

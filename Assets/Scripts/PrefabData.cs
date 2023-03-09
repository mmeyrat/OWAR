using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabData : MonoBehaviour
{
    private int tagAreaId;
    private int tagAreaSlotId;

    /**
    * Return the value of the tag area's id
    * 
    * @return tag area's id
    **/
    public int GetTagAreaId()
    {
        return this.tagAreaId;
    }

    /**
    * Return the value of the slot's id used by the prefab in the tag area 
    * 
    * @return slot's id 
    **/
    public int GetTagAreaSlotId()
    {
        return this.tagAreaSlotId;
    }
    
    /**
    * Set the tag area's id
    * 
    * @param id : the tag area id 
    **/
    public void SetTagAreaId(int id)
    {
        this.tagAreaId = id;
    }
    
    /**
    * Set the slot's id used by the prefab in the tag area
    * 
    * @param id : the slot id 
    **/
    public void SetTagAreaSlotId(int id)
    {
        this.tagAreaSlotId = id;
    }

}

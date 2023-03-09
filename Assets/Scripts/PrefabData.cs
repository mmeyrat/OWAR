using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabData : MonoBehaviour
{
    private int tagAreaId;
    private int tagAreaSlotId;

    public int GetTagAreaId()
    {
        return this.tagAreaId;
    }

    public int GetTagAreaSlotId()
    {
        return this.tagAreaSlotId;
    }

    public void SetTagAreaId(int id)
    {
        this.tagAreaId = id;
    }

    public void SetTagAreaSlotId(int id)
    {
        this.tagAreaSlotId = id;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagArea
{
    private string tag;
    private Vector3 position;
    private Vector3 scale;
    private Quaternion rotation;
    private bool[] slotsAvailable;

    public TagArea(string tag, Vector3 position, Vector3 scale, Quaternion rotation)
    {
        this.tag = tag;
        this.position = position;
        this.scale = scale;
        this.rotation = rotation;
        this.slotsAvailable = new bool[] { true, true, true, true };
    }

    /**
    * Return the tag of the tag area
    * 
    * @return tag area's tag
    **/
    public string GetTag()
    {
        return this.tag;
    }

    /**
    * Return the position values of the tag area
    * 
    * @return tag area's position
    **/
    public Vector3 GetPosition()
    {
        return this.position;
    }

    /**
    * Return the scale values of the tag area
    * 
    * @return tag area's scale
    **/
    public Vector3 GetScale()
    {
        return this.scale;
    }

    /**
    * Return the rotation values of the tag area
    * 
    * @return tag area's rotation
    **/
    public Quaternion GetRotation()
    {
        return this.rotation;
    }

    /**
    * Return the availability of the specified slot in the tag area
    * 
    * @return the slot availability
    **/
    public bool GetSlotAvailability(int id)
    {
        bool availability = false;

        if (id >= 0 && id < slotsAvailable.Length)
        {
            availability = slotsAvailable[id];
        }

        return availability;
    }

    /**
    * Change the availabiliy of the specified slot in the tag area
    * 
    * @param id : tag area's slot
    * @param availability : slot's availability
    **/
    public void SetSlotAvailability(int id, bool availability)
    {
        if (id >= 0 && id < slotsAvailable.Length)
        {
            slotsAvailable[id] = availability;
        }
    }
}

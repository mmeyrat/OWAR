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

    public string GetTag()
    {
        return this.tag;
    }

    public Vector3 GetPosition()
    {
        return this.position;
    }

    public Vector3 GetScale()
    {
        return this.scale;
    }

    public Quaternion GetRotation()
    {
        return this.rotation;
    }

    public bool GetSlotAvailability(int id)
    {
        bool availability = false;

        if (id >= 0 && id < slotsAvailable.Length)
        {
            availability = slotsAvailable[id];
        }

        return availability;
    }

    public void SetSlotAvailability(int id, bool availability)
    {
        if (id >= 0 && id < slotsAvailable.Length)
        {
            slotsAvailable[id] = availability;
        }
    }
}

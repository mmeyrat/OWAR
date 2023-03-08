using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagArea
{
    private string tag;
    private Vector3 position;
    private Vector3 scale;

    public TagArea(string tag, Vector3 position, Vector3 scale)
    {
        this.tag = tag;
        this.position = position;
        this.scale = scale;
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
}

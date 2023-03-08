using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DisplayDropdown : MonoBehaviour
{
    public Dropdown dropdownList;

    /**
    * Update is called at each frame update
    **/
    void Update() 
    {
        dropdownList.Show();
        Debug.Log(TagSceneHandler.GetTagAreaList()[0].GetPosition());
        Debug.Log(TagSceneHandler.GetTagAreaList()[1].GetPosition());
    }
}

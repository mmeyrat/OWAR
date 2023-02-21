using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class DisplayDropdown : MonoBehaviour
{
    public Dropdown dropdownList;

    void Update() {
        dropdownList.Show();
    }
}

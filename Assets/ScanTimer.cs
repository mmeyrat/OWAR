using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScanTimer : MonoBehaviour
{

    public GameObject mixedRealityPlayspace; 
    private Text scanIndicator;

    void Start() 
    {
        scanIndicator = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {   
        float time = mixedRealityPlayspace.GetComponent<Heatmap>().GetTimeRemaining();
        if (time <= 0) {
            scanIndicator.text = ""; 
        } else {
            scanIndicator.text = "Scan finished in " + (int)time + " s";
        }
    }
}

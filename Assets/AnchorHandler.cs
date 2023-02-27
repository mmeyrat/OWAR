using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class AnchorHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject anchoredObject;

    public GameObject switchModeTrigger;
    private Color oldColor = Color.red; // set to red in order to detect if bad initiate


    // To switch anchor mode of anchoredObject and change anchor button color as feedback
    public void switchMode()
    {
        // TODO FIX this forces to name anchor button "AnchorButton" and not sure it's correct
        // TODO FIX this forces to use a button to switch anchor mode
        
        // Variables
        var comp = anchoredObject.GetComponent<SolverHandler>(); // component that handle view anchor mode, so handle follow movement transformation
        Color triggerColor = switchModeTrigger.GetComponent<Image>().color; // anchor trigger's color

        // Register old button color to reset it later
        // oldButtonColor = buttonColor;

        if (comp.enabled) // if object is view anchored
        {
            // Switch to world anchor mode
            comp.enabled = false;

            //Change "anchor mode" button color to feedback
            switchModeTrigger.GetComponent<Image>().color = oldColor;

        }
        else // if object is world anchored
        {
            // Switch to view anchor mode
            comp.enabled = true;

            //Change "anchor mode" button color to feedback
            oldColor = triggerColor;
            switchModeTrigger.GetComponent<Image>().color = Color.green;
        }
    }
}

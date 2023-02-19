using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject go;

    public void switchMode()
    {
        var comp = go.GetComponent<SolverHandler>();

        /*if (comp.enabled)
        {
            comp.enabled = false;
        }
        else
        {
            comp.enabled = true;
        }*/

        comp.enabled = !comp.enabled;
    }
}

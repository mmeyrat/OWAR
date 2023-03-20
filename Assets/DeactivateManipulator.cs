using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeactivateManipulator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Element to disable when the user interacts with it
    public GameObject objectToDisable;

    /**
    * Check if the pointer enters the current element
    *
    * @param eventData : data of the occured event 
    **/
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Disable object manipulator
        DisableManipulator();
    }

    /**
    * Check if the pointer leaves the current element
    *
    * @param eventData : data of the occured event 
    **/
    public void OnPointerExit(PointerEventData eventData)
    {
        // (Re)enable object manipulator
        EnableManipulator();
    }

    public void DisableManipulator()
    {
        objectToDisable.GetComponent<ObjectManipulator>().enabled = false;
    }

    public void EnableManipulator()
    {
        objectToDisable.GetComponent<ObjectManipulator>().enabled = true;
    }

}

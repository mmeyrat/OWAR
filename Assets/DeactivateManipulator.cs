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
        DisableManipulator();
    }

    /**
    * Check if the pointer leaves the current element
    *
    * @param eventData : data of the occured event 
    **/
    public void OnPointerExit(PointerEventData eventData)
    {
        EnableManipulator();
    }
    
    /**
    * Disable object manipulator
    **/
    public void DisableManipulator()
    {
        objectToDisable.GetComponent<ObjectManipulator>().enabled = false;
    }

    /**
    * (Re)enable object manipulator
    **/
    public void EnableManipulator()
    {
        objectToDisable.GetComponent<ObjectManipulator>().enabled = true;
    }

}

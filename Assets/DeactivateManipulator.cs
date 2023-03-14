using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeactivateManipulator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Element which is partially disable when scrollbar is used
    public GameObject objectToDisable;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Disable object manipulator when the scrollbar is used
        var manipulatorComp = objectToDisable.GetComponent<ObjectManipulator>();
        manipulatorComp.enabled = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // (Re)enable object manipulator when the scrollbar have been used
        var manipulatorComp = objectToDisable.GetComponent<ObjectManipulator>();
        manipulatorComp.enabled = true;
    }

}

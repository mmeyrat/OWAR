using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollbarHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Start is called before the first frame update

    // Element which is partially disable when scrollbar is used
    public GameObject ElementToDisable;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Disable object manipulator when the scrollbar is used
        var manipulatorComponent = ElementToDisable.GetComponent<ObjectManipulator>();
        manipulatorComponent.enabled = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // (Re)enable object manipulator when the scrollbar have been used
        var manipulatorComponent = ElementToDisable.GetComponent<ObjectManipulator>();
        manipulatorComponent.enabled = true;
    }

}

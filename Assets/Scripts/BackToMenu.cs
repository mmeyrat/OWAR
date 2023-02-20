using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    /**
    * Return to the main menu
    **/
    public void Back() 
    {
        DropdownHandler.HasBeenDisplayed();
        SceneManager.LoadScene("Menu");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public void back() {
        DropdownHandler.hasBeenDisplayed();
        SceneManager.LoadScene("Menu");
    }
}

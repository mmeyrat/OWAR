using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeatmapLauncher : MonoBehaviour
{
    
    /**
    * Method to start timer at the first launch
    **/
    public void LaunchMainScene() {
        SceneManager.LoadScene("MainScene");
    }
}

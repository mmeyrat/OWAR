using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class MainSceneHandlerTests
    {
        string correctFileName = "Giorno.png";
        string incorrectFileName = "Girono.png";

        [SetUp]
        public void Setup()
        {
            SceneManager.LoadScene("MainScene");
        }

        [UnityTest]
        public IEnumerator SelectFileCorrectlyTest()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject menu = GameObject.Find("Menu");
            MainSceneHandler msh = menu.GetComponent<MainSceneHandler>();

            GameObject go = new GameObject();
            Text txt = go.AddComponent<Text>();
            txt.text = correctFileName;

            string oldSelectedFiles = msh.selectedFiles.text;
            msh.FileSelector(txt);
            
            Assert.AreNotEqual(oldSelectedFiles, msh.selectedFiles.text);
        }

        [UnityTest]
        public IEnumerator SelectFileIncorrectlyTest()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject menu = GameObject.Find("Menu");
            MainSceneHandler msh = menu.GetComponent<MainSceneHandler>();

            GameObject go = new GameObject();
            Text txt = go.AddComponent<Text>();
            txt.text = incorrectFileName;

            string oldSelectedFiles = msh.selectedFiles.text;
            msh.FileSelector(txt);
            
            Assert.AreEqual(oldSelectedFiles, msh.selectedFiles.text);
        }

        [UnityTest]
        public IEnumerator VisualizeCorrectlyTest()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject menu = GameObject.Find("Menu");
            MainSceneHandler msh = menu.GetComponent<MainSceneHandler>();

            GameObject go = new GameObject();
            Text txt = go.AddComponent<Text>();
            txt.text = correctFileName;

            msh.FileSelector(txt);
            int oldObjectNb = GameObject.FindObjectsOfType(typeof(GameObject)).Length;
            msh.Visualize();

            Assert.AreNotEqual(GameObject.FindObjectsOfType(typeof(GameObject)).Length, oldObjectNb);
        }

        [UnityTest]
        public IEnumerator VisualizeIncorrectlyTest()
        {
            yield return new WaitForSeconds(0.1f);
            GameObject menu = GameObject.Find("Menu");
            MainSceneHandler msh = menu.GetComponent<MainSceneHandler>();

            GameObject go = new GameObject();
            Text txt = go.AddComponent<Text>();
            txt.text = incorrectFileName;

            msh.FileSelector(txt);
            int oldObjectNb = GameObject.FindObjectsOfType(typeof(GameObject)).Length;
            msh.Visualize();

            Assert.AreNotEqual(GameObject.FindObjectsOfType(typeof(GameObject)).Length, oldObjectNb);
        }

    }
}

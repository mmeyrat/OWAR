using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using System.Text;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class ChangePageTests 
    {   
        [SetUp]
        public void Setup()
        {
            SceneManager.LoadScene("MainScene");
        }

        [UnityTest]
        public IEnumerator ChangeToNextPageCorrectlyTest()
        {
            yield return new WaitForSeconds(0.1f);
            string filePath = "Assets/StreamingAssets/SBR.txt";
            GameObject prefab = Object.Instantiate(Resources.Load("TextPrefab")) as GameObject;
            TMP_Text txt = prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            
            prefab.GetComponent<DisplayText>().SetTextObject(txt);
            prefab.GetComponent<DisplayText>().SetFileName(filePath);
            prefab.GetComponent<ChangePage>().SetObj(txt);
            
            yield return new WaitForSeconds(0.1f);
            int oldCurrentPage = txt.pageToDisplay;
            prefab.GetComponent<ChangePage>().ChangeToNextPage();

            Assert.IsTrue(oldCurrentPage < txt.pageToDisplay);
        }

        [UnityTest]
        public IEnumerator ChangeToNextPageIncorrectlyTest()
        {
            yield return new WaitForSeconds(0.1f);
            string filePath = "Assets/StreamingAssets/LoremIpsum.txt";
            GameObject prefab = Object.Instantiate(Resources.Load("TextPrefab")) as GameObject;
            TMP_Text txt = prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            
            prefab.GetComponent<DisplayText>().SetTextObject(txt);
            prefab.GetComponent<DisplayText>().SetFileName(filePath);
            prefab.GetComponent<ChangePage>().SetObj(txt);
            
            yield return new WaitForSeconds(0.1f);
            int oldCurrentPage = txt.pageToDisplay;
            prefab.GetComponent<ChangePage>().ChangeToNextPage();

            Assert.IsTrue(oldCurrentPage >= txt.pageToDisplay);
        }

        [UnityTest]
        public IEnumerator ChangeToPreviousPageCorrectlyTest()
        {
            yield return new WaitForSeconds(0.1f);
            string filePath = "Assets/StreamingAssets/SBR.txt";
            GameObject prefab = Object.Instantiate(Resources.Load("TextPrefab")) as GameObject;
            TMP_Text txt = prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            txt.pageToDisplay = 2;
            
            prefab.GetComponent<DisplayText>().SetTextObject(txt);
            prefab.GetComponent<DisplayText>().SetFileName(filePath);
            prefab.GetComponent<ChangePage>().SetObj(txt);
            
            yield return new WaitForSeconds(0.1f);
            int oldCurrentPage = txt.pageToDisplay;
            prefab.GetComponent<ChangePage>().ChangeToPreviousPage();

            Assert.IsTrue(oldCurrentPage > txt.pageToDisplay);
        }

        [UnityTest]
        public IEnumerator ChangeToPreviousPageIncorrectlyTest()
        {
            yield return new WaitForSeconds(0.1f);
            string filePath = "Assets/StreamingAssets/LoremIpsum.txt";
            GameObject prefab = Object.Instantiate(Resources.Load("TextPrefab")) as GameObject;
            TMP_Text txt = prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            
            prefab.GetComponent<DisplayText>().SetTextObject(txt);
            prefab.GetComponent<DisplayText>().SetFileName(filePath);
            prefab.GetComponent<ChangePage>().SetObj(txt);
            
            yield return new WaitForSeconds(0.1f);
            int oldCurrentPage = txt.pageToDisplay;
            prefab.GetComponent<ChangePage>().ChangeToPreviousPage();

            Assert.IsTrue(oldCurrentPage <= txt.pageToDisplay);
        }
    }
}

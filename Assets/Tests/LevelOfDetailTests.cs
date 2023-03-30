using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;
using UnityEngine.UI;

namespace Tests
{
    public class LevelOfDetailTests
    {
        GameObject mainCamera;

        [SetUp]
        public void Setup()
        {
            SceneManager.LoadScene("MainScene");
        }

        [UnityTest]
        public IEnumerator TextDetailsChangeCorrectly()
        {
            yield return new WaitForSeconds(0.1f);
            string filePath = "Assets/StreamingAssets/SBR.txt";
            GameObject prefab = Object.Instantiate(Resources.Load("TextPrefab")) as GameObject;
            TMP_Text txt = prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            
            prefab.GetComponent<DisplayText>().SetTextObject(txt);
            prefab.GetComponent<DisplayText>().SetFileName(filePath);

            float oldFontSize = txt.fontSize;

            mainCamera = GameObject.Find("Main Camera");
            mainCamera.transform.position = new Vector3(1, 1, 1);
            yield return new WaitForSeconds(0.1f);

            Assert.AreNotEqual(oldFontSize, txt.fontSize);
        }

        [UnityTest]
        public IEnumerator TextDetailsChangeIncorrectly()
        {
            yield return new WaitForSeconds(0.1f);
            string filePath = "Assets/StreamingAssets/SBR.txt";
            GameObject prefab = Object.Instantiate(Resources.Load("TextPrefab")) as GameObject;
            TMP_Text txt = prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>();
            
            prefab.GetComponent<DisplayText>().SetTextObject(txt);
            prefab.GetComponent<DisplayText>().SetFileName(filePath);

            float oldFontSize = txt.fontSize;

            mainCamera = GameObject.Find("Main Camera");
            mainCamera.transform.position = new Vector3(0.001f, 0.001f, 0.001f);
            yield return new WaitForSeconds(0.1f);

            Assert.AreEqual(oldFontSize, txt.fontSize);
        }

        [UnityTest]
        public IEnumerator ImageDetailsChangeCorrectly()
        {
            yield return new WaitForSeconds(0.1f);
            string filePath = "Assets/StreamingAssets/Giorno.png";
            GameObject prefab = Object.Instantiate(Resources.Load("ImagePrefab")) as GameObject;
            RawImage ri = prefab.transform.GetChild(0).GetChild(0).GetComponent<RawImage>();
            
            prefab.GetComponent<DisplayImage>().SetImageObject(ri);
            prefab.GetComponent<DisplayImage>().SetFileName(filePath);

            Vector2 oldDeltaSize = ri.GetComponent<RectTransform>().sizeDelta;

            mainCamera = GameObject.Find("Main Camera");
            mainCamera.transform.position = new Vector3(1, 1, 1);
            yield return new WaitForSeconds(0.1f);

            Assert.AreNotEqual(oldDeltaSize, ri.GetComponent<RectTransform>().sizeDelta);
        }

        [UnityTest]
        public IEnumerator ImageDetailsChangeIncorrectly()
        {
            yield return new WaitForSeconds(0.1f);
            string filePath = "Assets/StreamingAssets/Giorno.png";
            GameObject prefab = Object.Instantiate(Resources.Load("ImagePrefab")) as GameObject;
            RawImage ri = prefab.transform.GetChild(0).GetChild(0).GetComponent<RawImage>();
            
            prefab.GetComponent<DisplayImage>().SetImageObject(ri);
            prefab.GetComponent<DisplayImage>().SetFileName(filePath);

            Vector2 oldDeltaSize = ri.GetComponent<RectTransform>().sizeDelta;

            mainCamera = GameObject.Find("Main Camera");
            mainCamera.transform.position = new Vector3(0.001f, 0.001f, 0.001f);
            yield return new WaitForSeconds(0.1f);

            Assert.AreEqual(oldDeltaSize, ri.GetComponent<RectTransform>().sizeDelta);
        }
    }
}

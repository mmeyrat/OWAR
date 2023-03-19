using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class TagSceneHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            string cameraName = "Main Camera";

            GameObject go1 = new GameObject(cameraName);
            Camera cam = go1.AddComponent<Camera>();
        }

        [UnityTest]
        public IEnumerator ChangeToMenuSceneCorrectlyTest()
        {
            string mainScene = "MainScene";
            GameObject go2 = new GameObject();
            TagSceneHandler tsh = go2.AddComponent<TagSceneHandler>();
            tsh.buttonMenu = new GameObject();

            tsh.SwitchToMenuScene();
            yield return new WaitForSeconds(0.1f);
            
            string activeSceneName = SceneManager.GetActiveScene().name;

            Assert.IsTrue(activeSceneName == mainScene);
        }

        [Test]
        public void GenerateTagAreaCorrectlyTest()
        {
            string tag = "TagArea";
            GameObject go2 = new GameObject();
            TagSceneHandler tsh = go2.AddComponent<TagSceneHandler>();
            tsh.buttonMenu = new GameObject();
            
            int oldCount = tsh.GetTempTagAreaList().Count;

            tsh.Start();
            tsh.GenerateTagArea(tag);

            int newCount = tsh.GetTempTagAreaList().Count;

            Assert.IsTrue(newCount > oldCount);
        }
    }
}

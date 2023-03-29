using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;
using System.Text;
using TMPro;

namespace Tests
{
    public class DisplayTextTests
    {
        string errorMessage = "Loading error.";

        [Test]
        public void LoadTextCorrectlyTest()
        {
            DisplayText dt = new DisplayText();
            GameObject go = new GameObject();
            TMP_Text tmp = go.AddComponent<TextMeshPro>();

            dt.SetTextObject(tmp);
            dt.SetFileName("Assets/StreamingAssets/LoremIpsum.txt");
            
            dt.LoadText();

            Assert.AreNotEqual(dt.GetTextObject().text, errorMessage);
        }

        [Test]
        public void LoadTextIncorrectlyTest()
        {
            DisplayText dt = new DisplayText();
            GameObject go = new GameObject();
            TMP_Text tmp = go.AddComponent<TextMeshPro>();

            dt.SetTextObject(tmp);
            dt.SetFileName("Assets/StreamingAssets/IpsumLorem.txt");
            
            dt.LoadText();

            Assert.AreEqual(dt.GetTextObject().text, errorMessage);
        }

        [Test]
        public void FileNameSetterIsCorrectTest()
        {
            DisplayText rt = new DisplayText();
            string fn = "Assets/StreamingAssets/LoremIpsum.txt";

            rt.SetFileName(fn);

            Assert.AreEqual(fn, rt.GetFileName());
        }
    }
}

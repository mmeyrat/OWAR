using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;


namespace Tests
{
    public class DisplayTextTests
    {
        [Test]
        public void ReadCorrectlyTextFromFileTest()
        {
            string filePath = "Assets/StreamingAssets/LoremIpsum.txt";
            string textContent = File.ReadAllText(filePath);

            string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. \nDuis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            Assert.AreEqual(textContent, text);
        }

        [Test]
        public void ReadIncorrectlyTextFromFileTest()
        {
            string filePath = "Assets/StreamingAssets/LoremIpsum.txt";
            string textContent = File.ReadAllText(filePath);

            string text = "This text is incorrect.";

            Assert.AreNotEqual(textContent, text);
        }

        [Test]
        public void FileNameSetterIsCorrectTest()
        {
            DisplayText rt = new DisplayText();
            string fn = "Assets/StreamingAssets/Giorno.png";

            rt.SetFileName(fn);

            Assert.AreEqual(fn, rt.GetFileName());
        }
    }
}

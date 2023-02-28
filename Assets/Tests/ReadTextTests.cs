using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;


namespace Tests
{
    public class ReadTextTests
    {
        [Test]
        public void ReadCorrectlyTextFromFileTest()
        {
            string filePath = "Assets/Texts/test.txt";
            string textContent = File.ReadAllText(filePath);

            string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. \r\nDuis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

            Assert.AreEqual(textContent, text);
        }

        [Test]
        public void ReadIncorrectlyTextFromFileTest()
        {
            string filePath = "Assets/Texts/test.txt";
            string textContent = File.ReadAllText(filePath);

            string text = "This text is incorrect.";

            Assert.AreNotEqual(textContent, text);
        }
/*
        [Test]
        public void FormatTextCorrectlyTest()
        {
            ReadText rt = new ReadText();

            string textToFormat = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            string correctText = "Lorem ipsum dolor sit\r\n amet, consectetur a\r\ndipiscing elit, sed \r\ndo eiusmod tempor in\r\ncididunt ut labore e\r\nt dolore magna aliqu\r\na.";

            Assert.AreEqual(rt.FormatText(textToFormat), correctText);
        }

        [Test]
        public void FormatTextIncorrectlyTest()
        {
            ReadText rt = new ReadText();

            string textToFormat = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
            string incorrectText = "Lor\r\nem ipsum dolor sit\r\n amet, co\r\nnsectetur a\r\ndipiscing elit, sed \r\ndo eius\r\nmod tempor in\r\ncididunt ut labore e\r\nt dolore mag\r\nna aliqu\r\na.";

            Assert.AreNotEqual(rt.FormatText(textToFormat), incorrectText);
        }*/
    }
}

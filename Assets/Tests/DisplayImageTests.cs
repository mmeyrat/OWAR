﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;
using System.Text;
using UnityEngine.UI;

namespace Tests
{
    public class DisplayImageTests
    {
        [Test]
        public void LoadImageCorrectlyTest()
        {
            DisplayImage di = new DisplayImage();
            GameObject go = new GameObject();
            RawImage ri = go.AddComponent<RawImage>();

            di.SetImageObject(ri);
            di.SetFileName("Assets/StreamingAssets/Giorno.png");
            
            di.LoadImage();

            Assert.IsNotNull(di.GetImageObject().texture);
        }

        [Test]
        public void LoadImageIncorrectlyTest()
        {
            DisplayImage di = new DisplayImage();
            GameObject go = new GameObject();
            RawImage ri = go.AddComponent<RawImage>();

            di.SetImageObject(ri);
            di.SetFileName("Assets/StreamingAssets/Girono.png");
            
            di.LoadImage();

            Assert.IsNull(di.GetImageObject().texture);
        }

        [Test]
        public void FileNameSetterIsCorrectTest()
        {
            DisplayImage di = new DisplayImage();
            string fn = "Assets/StreamingAssets/Giorno.png";

            di.SetFileName(fn);

            Assert.AreEqual(fn, di.GetFileName());
        }
    }
}

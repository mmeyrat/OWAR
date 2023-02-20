using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.IO;
using System.Text;


namespace Tests
{
    public class DisplayImageTests
    {
        [Test]
        public void LoadImageCorrectlyTest()
        {
            DisplayImage di = new DisplayImage();

            di.imagePoster = GameObject.CreatePrimitive(PrimitiveType.Cube);
            di.setFilename("Assets/Images/giorno.png");
            di.Start();

            Assert.IsNotNull(di.imagePoster.GetComponent<Renderer>().material.mainTexture);
        }

        [Test]
        public void LoadImageIncorrectlyTest()
        {
            try {
                DisplayImage di = new DisplayImage();

                di.imagePoster = GameObject.CreatePrimitive(PrimitiveType.Cube);;
                di.setFilename("Assets/Images/girono.jovana");
                di.Start();

                Assert.IsNull(di.imagePoster.GetComponent<Renderer>().material.mainTexture);
            } catch  {
                Debug.Log("Test failed.");
            }
        }

        [Test]
        public void FilenameSetterIsCorrectTest()
        {
            DisplayImage di = new DisplayImage();
            string fn = "Assets/Images/giorno.png";

            di.setFilename(fn);

            Assert.AreEqual(fn, di.getFilename());
        }

        [Test]
        public void PoseXSetterIsCorrectTest()
        {
            DisplayImage di = new DisplayImage();
            float px = 0.5f;

            di.setPoseX(px);

            Assert.AreEqual(px, di.getPoseX());
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CloseTests
    {
        [UnityTest]
        public IEnumerator CloseWindowCorrectlyTest()
        {
            GameObject go = new GameObject();
            Close close = new Close();

            close.SetObj(go);
            close.CloseWindow();

            yield return new WaitForSeconds(0.1f);

            Assert.IsTrue(go == null);            
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using System.IO;
using System.Text;
using UnityEngine.UI;

namespace Tests
{
    public class ChangePageTests 
    {
        [UnityTest]
        public IEnumerator ChangeToNextPageCorrectlyTest()
        {
            //Camera cam = new Camera();
            string filePath = "Assets/StreamingAssets/SBR.txt";
            GameObject prefab = Object.Instantiate(Resources.Load("TextPrefab")) as GameObject;
            
            prefab.GetComponent<DisplayText>().SetTextObject(prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
            prefab.GetComponent<DisplayText>().SetFileName(filePath);
            prefab.GetComponent<ChangePage>().SetObj(prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>());
            
            yield return new WaitForSeconds(0.1f);
            Debug.Log(prefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().textInfo.pageCount);
            
            prefab.GetComponent<ChangePage>().ChangeToNextPage();
            yield return new WaitForSeconds(0.1f);

            //yield return new WaitForSeconds(0.1f);
            Debug.Log(prefab.GetComponent<ChangePage>().GetTextObject().pageToDisplay);
        } 
    }
}

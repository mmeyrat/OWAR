using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangePage : MonoBehaviour
{
    private TMP_Text textWithPages;

    /**
    * Set the file to change the pages
    **/
    public void SetObj(TMP_Text obj) 
    {
        textWithPages = obj;
    }

    /**
    * Set the current page to the next one
    **/
    public void ChangeToNextPage() 
    {
        if (textWithPages.pageToDisplay < textWithPages.textInfo.pageCount)
        {
            textWithPages.pageToDisplay++;
        }
        else 
        {
            textWithPages.pageToDisplay = 1;
        }
    }
}

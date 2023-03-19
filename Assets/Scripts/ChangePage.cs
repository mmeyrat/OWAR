using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangePage : MonoBehaviour
{
    private TMP_Text textWithPages;
    
    private int minPage = 1;

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
            textWithPages.pageToDisplay = minPage;
        }
    }

    /**
    * Set the current page to the previous one
    **/
    public void ChangeToPreviousPage()
    {
        if (textWithPages.pageToDisplay > minPage)
        {
            textWithPages.pageToDisplay--;
        }
        else
        {
            textWithPages.pageToDisplay = textWithPages.textInfo.pageCount;
        }
    }

    /**
    * Return to text object from which its page are changeable
    *
    * @return text object 
    **/
    public TMP_Text GetTextObject()
    {
        return this.textWithPages;
    }
}

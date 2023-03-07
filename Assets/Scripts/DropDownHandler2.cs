using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.UI.CoroutineTween;
using System;

public class DropdownHandler2 : MonoBehaviour
{
    // To deploy on HoloLens use this path (works also on Unity):
    private static string path = Application.streamingAssetsPath;
    private static Dictionary<string, bool> choosenFiles = new Dictionary<string, bool>();
    private static string[] fileEntries;
    private static bool alreadyDisplayed = false;

    /**
    * Start is called before the first frame update
    **/
    void Start()
    {
        //Dropdown dropdown = transform.GetComponent<Dropdown>();
        //dropdown.options.Clear();

        // Display all files names available in the dropdown list
        fileEntries = Directory.GetFiles(path);

        foreach (string file in fileEntries)
        {
            if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg") || file.EndsWith(".txt"))
            {
                //dropdown.options.Add(new Dropdown.OptionData() { text = file.Substring(path.Length + 1) });
                AddItemToList(file);

                if (alreadyDisplayed == false)
                {
                    choosenFiles.Add(file.Substring(path.Length + 1), false);
                }
            }
        }

        // Get the number of options
        //print(dropdown.options.Count);
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////
    /// </summary>

    private GameObject m_DropDownTemplate;

    // Template used to create the dropdown.
    [SerializeField]
    private RectTransform m_Template;

    private List<DropdownItem> m_Items = new List<DropdownItem>();

    public List<OptionData> options
    {
        get { return null; /*m_Options.options;*/ }
        set { /*m_Options.options = value; RefreshShownValue();*/ }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file"></param>
    private void AddItemToList(string file)
    {
        // Get root canvas
        var rootCanvas = this.transform.GetChild(0);

        // Instantiate the drop-down template
        m_DropDownTemplate = (GameObject)Instantiate(m_Template.gameObject);
        m_DropDownTemplate.name = "File List";
        m_DropDownTemplate.SetActive(true);

        // Make drop-down RectTransform have same values as original.
        RectTransform dropdownRectTransform = m_DropDownTemplate.transform as RectTransform;
        dropdownRectTransform.SetParent(m_Template.transform.parent, false);

        // Instantiate the drop-down list items

        // Find the dropdown item and disable it.
        DropdownItem itemTemplate = m_DropDownTemplate.GetComponentInChildren<DropdownItem>();

        GameObject content = itemTemplate.rectTransform.parent.gameObject;
        RectTransform contentRectTransform = content.transform as RectTransform;
        itemTemplate.rectTransform.gameObject.SetActive(true);

        // Get the rects of the dropdown and item
        Rect dropdownContentRect = contentRectTransform.rect;
        Rect itemTemplateRect = itemTemplate.rectTransform.rect;

        // Calculate the visual offset between the item's edges and the background's edges
        Vector2 offsetMin = itemTemplateRect.min - dropdownContentRect.min + (Vector2)itemTemplate.rectTransform.localPosition;
        Vector2 offsetMax = itemTemplateRect.max - dropdownContentRect.max + (Vector2)itemTemplate.rectTransform.localPosition;
        Vector2 itemSize = itemTemplateRect.size;

        m_Items.Clear();

        Toggle prev = null;
        for (int i = 0; i < options.Count; ++i)
        {
            OptionData data = options[i];
            DropdownItem item = AddItem(data, m_Value == i, itemTemplate, m_Items);
            if (item == null)
                continue;

            // Automatically set up a toggle state change listener
            item.toggle.isOn = m_Value == i;
            item.toggle.onValueChanged.AddListener(x => OnSelectItem(item.toggle));

            // Select current option
            if (item.toggle.isOn)
                item.toggle.Select();

            // Automatically set up explicit navigation
            if (prev != null)
            {
                Navigation prevNav = prev.navigation;
                Navigation toggleNav = item.toggle.navigation;
                prevNav.mode = Navigation.Mode.Explicit;
                toggleNav.mode = Navigation.Mode.Explicit;

                prevNav.selectOnDown = item.toggle;
                prevNav.selectOnRight = item.toggle;
                toggleNav.selectOnLeft = prev;
                toggleNav.selectOnUp = prev;

                prev.navigation = prevNav;
                item.toggle.navigation = toggleNav;
            }
            prev = item.toggle;
        }

        // Reposition all items now that all of them have been added
        Vector2 sizeDelta = contentRectTransform.sizeDelta;
        sizeDelta.y = itemSize.y * m_Items.Count + offsetMin.y - offsetMax.y;
        contentRectTransform.sizeDelta = sizeDelta;

        float extraSpace = dropdownRectTransform.rect.height - contentRectTransform.rect.height;
        if (extraSpace > 0)
            dropdownRectTransform.sizeDelta = new Vector2(dropdownRectTransform.sizeDelta.x, dropdownRectTransform.sizeDelta.y - extraSpace);

        // Invert anchoring and position if dropdown is partially or fully outside of canvas rect.
        // Typically this will have the effect of placing the dropdown above the button instead of below,
        // but it works as inversion regardless of initial setup.
        Vector3[] corners = new Vector3[4];
        dropdownRectTransform.GetWorldCorners(corners);

        RectTransform rootCanvasRectTransform = rootCanvas.transform as RectTransform;
        Rect rootCanvasRect = rootCanvasRectTransform.rect;
        for (int axis = 0; axis < 2; axis++)
        {
            bool outside = false;
            for (int i = 0; i < 4; i++)
            {
                Vector3 corner = rootCanvasRectTransform.InverseTransformPoint(corners[i]);
                if ((corner[axis] < rootCanvasRect.min[axis] && !Mathf.Approximately(corner[axis], rootCanvasRect.min[axis])) ||
                    (corner[axis] > rootCanvasRect.max[axis] && !Mathf.Approximately(corner[axis], rootCanvasRect.max[axis])))
                {
                    outside = true;
                    break;
                }
            }
            if (outside)
                RectTransformUtility.FlipLayoutOnAxis(dropdownRectTransform, axis, false, false);
        }

        for (int i = 0; i < m_Items.Count; i++)
        {
            RectTransform itemRect = m_Items[i].rectTransform;
            itemRect.anchorMin = new Vector2(itemRect.anchorMin.x, 0);
            itemRect.anchorMax = new Vector2(itemRect.anchorMax.x, 0);
            itemRect.anchoredPosition = new Vector2(itemRect.anchoredPosition.x, offsetMin.y + itemSize.y * (m_Items.Count - 1 - i) + itemSize.y * itemRect.pivot.y);
            itemRect.sizeDelta = new Vector2(itemRect.sizeDelta.x, itemSize.y);
        }

        // Fade in the popup
        AlphaFadeList(m_AlphaFadeSpeed, 0f, 1f);

        // Make drop-down template and item template inactive
        m_Template.gameObject.SetActive(false);
        itemTemplate.gameObject.SetActive(false);

        //m_Blocker = CreateBlocker(rootCanvas);
    }

    [SerializeField]
    private float m_AlphaFadeSpeed = 0.15f;

    private GameObject m_Blocker;

    [SerializeField]
    private int m_Value;

    // Change the value and hide the dropdown.
    private void OnSelectItem(Toggle toggle)
    {
        if (!toggle.isOn)
            toggle.isOn = true;

        int selectedIndex = -1;
        Transform tr = toggle.transform;
        Transform parent = tr.parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i) == tr)
            {
                // Subtract one to account for template child.
                selectedIndex = i - 1;
                break;
            }
        }

        if (selectedIndex < 0)
            return;

        m_Value = selectedIndex;
        Hide();
    }

    /// <summary>
    /// Create a blocker that blocks clicks to other controls while the dropdown list is open.
    /// </summary>
    /// <remarks>
    /// Override this method to implement a different way to obtain a blocker GameObject.
    /// </remarks>
    /// <param name="rootCanvas">The root canvas the dropdown is under.</param>
    /// <returns>The created blocker object</returns>
    protected virtual GameObject CreateBlocker(Canvas rootCanvas)
    {
        // Create blocker GameObject.
        GameObject blocker = new GameObject("Blocker");

        // Setup blocker RectTransform to cover entire root canvas area.
        RectTransform blockerRect = blocker.AddComponent<RectTransform>();
        blockerRect.SetParent(rootCanvas.transform, false);
        blockerRect.anchorMin = Vector3.zero;
        blockerRect.anchorMax = Vector3.one;
        blockerRect.sizeDelta = Vector2.zero;

        // Make blocker be in separate canvas in same layer as dropdown and in layer just below it.
        Canvas blockerCanvas = blocker.AddComponent<Canvas>();
        blockerCanvas.overrideSorting = true;
        Canvas dropdownCanvas = m_DropDownTemplate.GetComponent<Canvas>();
        blockerCanvas.sortingLayerID = dropdownCanvas.sortingLayerID;
        blockerCanvas.sortingOrder = dropdownCanvas.sortingOrder - 1;

        // Find the Canvas that this dropdown is a part of
        Canvas parentCanvas = null;
        Transform parentTransform = m_Template.parent;
        while (parentTransform != null)
        {
            parentCanvas = parentTransform.GetComponent<Canvas>();
            if (parentCanvas != null)
                break;

            parentTransform = parentTransform.parent;
        }

        // If we have a parent canvas, apply the same raycasters as the parent for consistency.
        if (parentCanvas != null)
        {
            Component[] components = parentCanvas.GetComponents<BaseRaycaster>();
            for (int i = 0; i < components.Length; i++)
            {
                Type raycasterType = components[i].GetType();
                if (blocker.GetComponent(raycasterType) == null)
                {
                    blocker.AddComponent(raycasterType);
                }
            }
        }
        else
        {
            // Add raycaster since it's needed to block.
            //GetOrAddComponent<GraphicRaycaster>(blocker);
        }


        // Add image since it's needed to block, but make it clear.
        Image blockerImage = blocker.AddComponent<Image>();
        blockerImage.color = Color.clear;

        // Add button since it's needed to block, and to close the dropdown when blocking area is clicked.
        Button blockerButton = blocker.AddComponent<Button>();
        blockerButton.onClick.AddListener(Hide);

        return blocker;
    }

    /// <summary>
    /// Hide the dropdown list. I.e. close it.
    /// </summary>
    public void Hide()
    {
        if (m_DropDownTemplate != null)
        {
            AlphaFadeList(m_AlphaFadeSpeed, 0f);

            // User could have disabled the dropdown during the OnValueChanged call.
            /*
            if (IsActive())
                StartCoroutine(DelayedDestroyDropdownList(m_AlphaFadeSpeed));
            */
        }
        /*
        if (m_Blocker != null)
            DestroyBlocker(m_Blocker);
        m_Blocker = null;
        Select();
        */
    }

    private void AlphaFadeList(float duration, float alpha)
    {
        CanvasGroup group = m_DropDownTemplate.GetComponent<CanvasGroup>();
        AlphaFadeList(duration, group.alpha, alpha);
    }

    private void AlphaFadeList(float duration, float start, float end)
    {
        if (end.Equals(start))
            return;

        /*
        FloatTween tween = new FloatTween { duration = duration, startValue = start, targetValue = end };
        tween.AddOnChangedCallback(SetAlpha);
        tween.ignoreTimeScale = true;
        m_AlphaTweenRunner.StartTween(tween);
        */
    }

    // Add a new drop-down list item with the specified values.
    private DropdownItem AddItem(OptionData data, bool selected, DropdownItem itemTemplate, List<DropdownItem> items)
    {
        // Add a new item to the dropdown.
        DropdownItem item = (DropdownItem)Instantiate(itemTemplate);
        item.rectTransform.SetParent(itemTemplate.rectTransform.parent, false);

        item.gameObject.SetActive(true);
        item.gameObject.name = "Item " + items.Count + (data.text != null ? ": " + data.text : "");

        if (item.toggle != null)
        {
            item.toggle.isOn = false;
        }

        // Set the item's data
        if (item.text)
            item.text.text = data.text;
        if (item.image)
        {
            item.image.sprite = data.image;
            item.image.enabled = (item.image.sprite != null);
        }

        items.Add(item);
        return item;
    }


    protected internal class DropdownItem : MonoBehaviour, IPointerEnterHandler, ICancelHandler
    {
        [SerializeField]
        private Text m_Text;
        [SerializeField]
        private Image m_Image;
        [SerializeField]
        private RectTransform m_RectTransform;
        [SerializeField]
        private Toggle m_Toggle;

        public Text text { get { return m_Text; } set { m_Text = value; } }
        public Image image { get { return m_Image; } set { m_Image = value; } }
        public RectTransform rectTransform { get { return m_RectTransform; } set { m_RectTransform = value; } }
        public Toggle toggle { get { return m_Toggle; } set { m_Toggle = value; } }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
            Dropdown dropdown = GetComponentInParent<Dropdown>();
            if (dropdown)
                dropdown.Hide();
        }
    }

    /// <summary>
    /// Class to store the text and/or image of a single option in the dropdown list.
    /// </summary>
    public class OptionData
    {
        [SerializeField]
        private string m_Text;
        [SerializeField]
        private Sprite m_Image;

        /// <summary>
        /// The text associated with the option.
        /// </summary>
        public string text { get { return m_Text; } set { m_Text = value; } }

        /// <summary>
        /// The image associated with the option.
        /// </summary>
        public Sprite image { get { return m_Image; } set { m_Image = value; } }

        public OptionData()
        {
        }

        public OptionData(string text)
        {
            this.text = text;
        }

        public OptionData(Sprite image)
        {
            this.image = image;
        }

        /// <summary>
        /// Create an object representing a single option for the dropdown list.
        /// </summary>
        /// <param name="text">Optional text for the option.</param>
        /// <param name="image">Optional image for the option.</param>
        public OptionData(string text, Sprite image)
        {
            this.text = text;
            this.image = image;
        }
    }

    //////////////////////////////////////////////////////////////////////


    /**
    * Return if a file has been already choosen 
    * 
    * @param file : the selected file     
    * @return true or false
    **/
    public static bool IsFileChoosen(string file)
    {
        return choosenFiles[file];
    }

    /**
    * Unselect every file to display the choosen ones
    **/
    public static void HasBeenDisplayed()
    {
        alreadyDisplayed = true;

        foreach (string file in fileEntries)
        {
            choosenFiles[file] = false;
        }
    }

    /**
    * Select a file to display
    * 
    * @param file : the file to select
    **/
    public static void SetToChoosen(string file)
    {
        choosenFiles[file] = !choosenFiles[file];
    }

    /**
    * Return the path of the directory where the files are located
    * 
    * @return directory path
    **/
    public static string GetPath()
    {
        return path;
    }

    /**
    * Return the files available to display
    * 
    * @return an array of files
    **/
    public static string[] GetFiles()
    {
        fileEntries = Directory.GetFiles(path);

        for (int i = 0; i < fileEntries.Length; i++)
        {
            fileEntries[i] = fileEntries[i].Substring(path.Length + 1);
        }

        return fileEntries;
    }
}

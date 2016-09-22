using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GUIManager : MonoBehaviour, IGUIBehavior
{
    [SerializeField]
    private List<GameObject> registered = new List<GameObject>();

    protected Dictionary<string, GameObject> elements = new Dictionary<string, GameObject>();
    protected Dictionary<string, GameObject> panels = new Dictionary<string, GameObject>();
    protected bool debug;
    protected bool init;

    [SerializeField]
    private string openTransitionName = "Open";
    [SerializeField]
    private string closedStateName = "Closed";
    [SerializeField]
    protected int openParameterId;
    public GameObject GameWindow;
    public GameObject VideoWindow;
    public GameObject AudioWindow;
    private Animator Open;
    private GameObject PreviouslySelected;

    [ContextMenu("Enable All Children")]
    public void EnableAll()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    [ContextMenu("Disable All Children")]
    public void DisableAll()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public bool CheckActive(string item)
    {
        if(elements[item] == null)
        {
            Debug.LogError("item doesnt exist: " + item);

            return false;
        }

        else
        {
            return elements[item].activeSelf;
        }
    }

    public virtual void Show(string item)
    {
        //Debug.Log("show " + item);

        if (elements[item].activeSelf == false && elements[item] != null)
        {
            elements[item].transform.SetAsLastSibling();

            elements[item].SetActive(true);

            
        }

        else
        {
            Debug.Log("could not show " + item);
        }
    }

    //this will only disable the object
    //the object that is disabled will handle popping itself off the menustate stack
    public void Hide(string item)
    {

        if (elements[item].activeSelf == true && elements[item] != null)
        {
            elements[item].transform.SetAsFirstSibling();

            elements[item].SetActive(false);

            
            
        }
        else
        {
            //Debug.Log("could not hide: " + item);
        }
    }

    /// <summary>
    /// setactive true this objects elements
    /// </summary>
    public void ShowAll()
    {
        foreach (KeyValuePair<string, GameObject> entry in elements)
        {
            if (entry.Value != null)
            {
                Show(entry.Key);
            }
        }

    }
    /// <summary>
    /// setactive false this objects elements
    /// </summary>
    public void HideAll()
    {
        foreach (KeyValuePair<string, GameObject> entry in elements)
        {
            if (entry.Value != null)
            {
                Hide(entry.Key);
            }
        }
    }

    public void EnableButtonInteraction(string item)
    {
        if (elements[item] != null)
        {
            // get all elements in this object
            Button[] buttons = elements[item].GetComponentsInChildren<Button>();

            // enable interaction on each one
            foreach(Button button in buttons)
            {
                button.interactable = true;
            }
        }
    }

    public void DisableButtonInteraction(string item)
    {
        if (elements[item] != null)
        {
            // get all elements in this object
            Button[] buttons = elements[item].GetComponentsInChildren<Button>();

            // disable interaction on each one
            foreach (Button button in buttons)
            {
                button.interactable = false;
            }
        }
    }

    public void EnableAllButtonInteraction()
    {
        foreach (KeyValuePair<string, GameObject> entry in elements)
        {
            // if GO has a button, enable interaction
            Button button = entry.Value.GetComponent<Button>();

            if(button != null)
            {
                button.interactable = true;
            }
        }
    }

    public void DisableAllButtonInteraction()
    {
        foreach (KeyValuePair<string, GameObject> entry in elements)
        {
            // if GO has a button, disable interaction
            Button button = entry.Value.GetComponent<Button>();

            if (button != null)
            {
                button.interactable = false;
            }
        }
    }

    protected virtual void Awake()
    {
        //all elements register at this point
        if (debug)
        {
            Debug.Log("Awake for " + gameObject.name + " Manager");
        }

    }

    protected virtual void OnEnable()
    {
        Debug.Log("enabling guimanager: " + gameObject.name);
        if (!init)
        {
            if (debug)
            {
                Debug.Log("Set init for " + gameObject.name);
            }

            init = true;
            openParameterId = Animator.StringToHash(openTransitionName);

            HideAll();
        }

        else
        {
            if (debug)
            {
                Debug.Log(name + " already init :: first OnEnable triggered");
            }
        }



    }

    protected virtual void OnDisable()
    {

    }

    public virtual void Register(GameObject ele)
    {
        if (debug)
        {
            Debug.Log("Register " + ele.name);
        }
        elements.Add(ele.name, ele);
        registered.Add(ele);

    }

    //when it goes disable pop it off
    public virtual void Unregister(GameObject ele)
    {

    }

    public bool HasRegisteredElement(string name)
    {
        return elements.ContainsKey(name);
    }
    /*
    #region MENU PANEL ACTIONS
    public void OpenPanel(Animator anim)
    {
        if (Open == anim)
            return;
        anim.gameObject.SetActive(true);
        var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;

        //anim.transform.SetAsLastSibling();

        CloseCurrent();

        PreviouslySelected = newPreviouslySelected;

        Open = anim;
        Open.SetBool(OpenParameterId, true);
    }

    public void OpenPanelWithoutClose(Animator anim)
    {
        if (Open == anim)
            return;
        anim.gameObject.SetActive(true);
        var newPreviouslySelected = EventSystem.current.currentSelectedGameObject;

        //anim.transform.SetAsLastSibling();

        PreviouslySelected = newPreviouslySelected;

        anim.SetBool(OpenParameterId, true);
    }

    public void CloseCurrent()
    {
        if (Open == null)
            return;
        Open.SetBool(OpenParameterId, false);
        StartCoroutine(DisablePanelDelayed(Open));
        Open = null;
    }
    
    public void CloseWindow(Animator anim)
    {
        if (!anim.gameObject.active)
        {
            return;
        }
        anim.SetBool(OpenParameterId, false);
        StartCoroutine(DisablePanelDelayed(anim));
    }

    public IEnumerator DisablePanelDelayed(Animator anim)
    {
        bool closedStateReached = false;
        bool wantToClose = true;
        while (!closedStateReached && wantToClose)
        {
            if (!anim.IsInTransition(0))
                closedStateReached = anim.GetCurrentAnimatorStateInfo(0).IsName(ClosedStateName);

            wantToClose = !anim.GetBool(OpenParameterId);

            yield return new WaitForEndOfFrame();
        }
        if (wantToClose)
        {
            if (anim.gameObject.tag == "Settings")
            {
                if (GameWindow.activeSelf)
                {
                    GameWindow.SetActive(false);
                }
                else if (GameWindow.activeSelf)
                {
                    VideoWindow.SetActive(false);
                }
                else if (GameWindow.activeSelf)
                {
                    AudioWindow.SetActive(false);
                }
            }
            anim.gameObject.SetActive(false);
        }

        Hide(anim.gameObject.name);

    }

    public void SetSelected()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    #endregion
    */
    #region ENABLE/DISABLE INTERACTION 
    // Keep a list of pointer input modules that we have disabled so that we can re-enable them
    List<PointerInputModule> disabled = new List<PointerInputModule>();

    int disableCount = 0;    // How many times has disable been called?

    public void Disable()
    {
        if (disableCount++ == 0)
        {
            UpdateState(false);
        }
    }

    public void Enable(bool enable)
    {
        if (!enable)
        {
            Disable();
            return;
        }
        if (--disableCount == 0)
        {
            UpdateState(true);
            if (disableCount > 0)
            {
                Debug.LogWarning("Warning UIDisableInput.Enable called more than Disable");
            }
        }
    }


    void UpdateState(bool enabled)
    {
        // First re-enable all systems
        for (int i = 0; i < disabled.Count; i++)
        {
            if (disabled[i])
            {
                disabled[i].enabled = true;
            }
        }

        disabled.Clear();

        EventSystem es = EventSystem.current;

        if (es == null) return;

        es.sendNavigationEvents = enabled;

        if (!enabled)
        {
            // Find all PointerInputModules and disable them
            PointerInputModule[] pointerInput = es.GetComponents<PointerInputModule>();
            if (pointerInput != null)
            {
                for (int i = 0; i < pointerInput.Length; i++)
                {
                    PointerInputModule pim = pointerInput[i];
                    if (pim.enabled)
                    {
                        pim.enabled = false;
                        // Keep a list of disabled ones
                        disabled.Add(pim);
                    }
                }
            }

            // Cause EventSystem to update it's list of modules
            es.enabled = false;
            es.enabled = true;
        }
    }
    #endregion


}

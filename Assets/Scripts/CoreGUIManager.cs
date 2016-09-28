using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CoreGUIManager : GUIManager, IGUIBehavior
{
    [SerializeField]
    private Camera guiCamera;

    public Camera GUICamera
    {
        get
        {
            return guiCamera;
        }
    }

    public enum MenuState
    {
        Active,
        Paused,

    }

    public MenuState CurrentState;

    

    [SerializeField]
    private GameObject endGameResultPrefab;

    [SerializeField]
    private GameObject endGameResults;

    private bool gameEndToggle;

    private static CoreGUIManager instance;

    public static CoreGUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<CoreGUIManager>();
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            //If I am the first instance, make me the Singleton
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }

        base.Awake();

        //elements["ToryDebug"].GetComponent<ToryDebug>().Init();
    }

    protected override void OnEnable()
    {
        base.OnEnable();



    }

    

    void Update()
    {
        
    }

    void OnLevelWasLoaded(int level)
    {
        Debug.Log("loaded level: " + Application.loadedLevelName);
        if(level == GameManager.Instance.MenuSceneNum)
        {
            if(gameEndToggle == true)
            {
                gameEndToggle = false;

                // add a results line for each user in userstats

                foreach(UserStatsToken stat in ServerManager.Instance.UserStats.Values)
                {
                    Debug.Log("Name: " + stat.DisplayName);

                    GameObject userResults = (GameObject)Instantiate(endGameResultPrefab, Vector3.zero, Quaternion.identity);
                    userResults.transform.SetParent(endGameResults.transform, false);

                    userResults.GetComponent<SetResultInfo>().SetInfo(stat);

                }

                // HIDE ALL OTHER PANELS FROM END OF LEVEL
                GameGUIManager.Instance.Hide("RespawnPanel");

                // display user stats screen
                Show("EndGameResults");
            }
        }
    }

    public void Pause(bool showMenu)
    {
        if (showMenu)
        {
            Show("Pause");
        }

        else
        {
            // disable input to entire gui?

            //GUIManager.instance.guiCamera.useKeyboard = false;
            //GUIManager.instance.guiCamera.useController = false;
            GUI.enabled = false;
        } 
        
        CurrentState = MenuState.Paused;

    }

    public void UnPause()
    {
        if (elements["Pause"].activeSelf == true)
        {
            Hide("Pause");
        }

        else
        {
            // re-enable input to entire gui?

            //GUIManager.instance.guiCamera.useKeyboard = true;
            //GUIManager.instance.guiCamera.useController = true;
            GUI.enabled = true;
        } 
        
        CurrentState = MenuState.Active;

    }

    public void SetActiveButton(string name)
    {
        //print("setactivebutton " + name);
        if (elements.ContainsKey(name))
        {
            // set active object

            //UICamera.selectedObject = _elements[name];
        }
    }

    public void SetEndGame(bool won)
    {
        gameEndToggle = true;
    }

    

    
}

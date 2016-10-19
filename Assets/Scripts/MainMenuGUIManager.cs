using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainMenuGUIManager : GUIManager, IGUIBehavior
{
    private static MainMenuGUIManager instance;

    public static MainMenuGUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<MainMenuGUIManager>();
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

        
    }

    void Start()
    {
        Debug.Log("showing stuff");
        Show("StartServer");
        Show("JoinServerPreset1");
        Show("JoinServerPreset2");
        Show("DisplayName");
        Hide("StartGame");

    }

    void Update()
    {

    }

    void OnLevelWasLoaded(int scene)
    {
        
    }

    public void BoltLoadGameScene()
    {


    }

    public void BoltUnloadGameScene()
    {

    }



}

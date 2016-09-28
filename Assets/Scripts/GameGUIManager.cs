using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameGUIManager : GUIManager, IGUIBehavior
{
    private static GameGUIManager instance;

    public static GameGUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameGUIManager>();
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

	void Start () 
    {
	
	}

	void Update () 
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

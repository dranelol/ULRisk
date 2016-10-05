using UnityEngine;
using System.Collections;
using System;

public class MapManager : MonoBehaviour
{
    [Serializable]
    public class MapRegionDatabase : SerializedDictionary<int, GameObject> { }

    /// <summary>
    /// record of user stats for this session
    /// </summary>
    [SerializeField]
    public MapRegionDatabase MapRegions = new MapRegionDatabase();

    private static MapManager instance;
    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<MapManager>();
            }
            DontDestroyOnLoad(instance.gameObject);
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            //If I am the first instance, make me the Singleton
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != instance)
            {
                Destroy(this.gameObject);
            }
        }

    }

    void Start ()
    {
	
	}
	
	void Update ()
    {
	
	}
}

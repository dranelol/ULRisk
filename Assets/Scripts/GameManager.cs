﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<GameManager>();
            }
            DontDestroyOnLoad(instance.gameObject);
            return instance;
        }
    }

    private bool netInit = false;
    
    public GameFSM FSM;

    public int PlayerID;

    [SerializeField]
    private List<Color> possibleColors = new List<Color>();

    /// <summary>
    /// Tracks player colors
    /// </summary>
    [Serializable]
    public class PlayerColorsDatabase : SerializedDictionary<CredentialToken, SerializedColor>
    {
        public PlayerColorsDatabase()
        {

        }

        public PlayerColorsDatabase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }

    [SerializeField]
    public PlayerColorsDatabase PlayerColors = new PlayerColorsDatabase();

    [SerializeField]
    public int LoadSceneNum;

    [SerializeField]
    public string LoadSceneName;

    [SerializeField]
    public int MenuSceneNum;

    [SerializeField]
    public string MenuSceneName;

    [SerializeField]
    public int MainGameSceneNum;

    [SerializeField]
    public string MainGameScene;

    public GameObject ServerManagerPrefab;
    public GameObject ClientManagerPrefab;
    public GameObject EndGameManagerPrefab;
    public GameObject GameTypeManagerPrefab;

    private bool started = false;

    private bool pickingRegion = false;

    private bool takingTurn = false;

    public bool LOCAL_TESTING = false;
    
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (LOCAL_TESTING == true)
            {
                //GameObject localWizard = (GameObject)Instantiate(LocalWizard, transform.position, Quaternion.identity);

                //LocalWizard wizard = localWizard.GetComponent<LocalWizard>();

                //wizard.GetComponent<LocalCharacterMovement>().MainCamera = Camera.main.gameObject;

                //Camera.main.gameObject.transform.parent = wizard.transform.Find("CamObj");

                //Camera.main.gameObject.GetComponent<UnityStandardAssets.Utility.FollowTarget>().target = wizard.transform.Find("CamObj");
            }
        }

        if(Input.GetKeyDown(KeyCode.X))
        {
            if (BoltNetwork.isServer)
            {

                ServerManager.Instance.GameEnd();
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (BoltNetwork.isServer)
            {
                //ClientManager.Instance.MyWizard.GetComponent<WizardAuraManager>().Add("SpawnAbility", ClientManager.Instance.MyWizard);
            }

        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (BoltNetwork.isServer)
            {
                //ClientManager.Instance.MyWizard.GetComponent<WizardAuraManager>().Remove("Test", ClientManager.Instance.MyWizard);
            }

        }
    }

    void OnLevelWasLoaded(int level)
    {
        if(level == MainGameSceneNum)
        {
            FSM.StartGame();
            GameGUIManager.Instance.Show("Continent1");
            GameGUIManager.Instance.Show("Continent2");
        }
    }

    /*

    /// <summary>
    /// If we are a client, then we raise an event, to notify the server 
    /// to spawn our wizard.
    /// If we are the server, we spawn the wizard normally, since the server
    /// if being authoratative anyway.
    /// </summary>
    public BoltEntity SpawnWizard( CredentialToken credentialsToken )
    {
        
        GameObject[] spawnLocations = GameObject.FindGameObjectsWithTag("Spawn");

        Vector3 spawnLocation = spawnLocations[UnityEngine.Random.Range(0, spawnLocations.Length)].transform.position;

        UserStatsToken userStatsToken = new UserStatsToken();

        //CredentialToken credentials = ClientManager.Instance.Credentials;

        userStatsToken.DisplayName = credentialsToken.DisplayName;
        userStatsToken.ThisWizardType = credentialsToken.ThisWizardType;

        Bolt.PrefabId wizardPrefabID = BoltPrefabs.ExampleWizard;

        Spellbook spellbook = null;

        switch(credentialsToken.ThisWizardType)
        {
            case WizardType.ExampleWizard:
                wizardPrefabID = BoltPrefabs.ExampleWizard;
                spellbook = transform.FindChild("Spellbook-ExampleWizard").GetComponent<Spellbook>();

                break;

            case WizardType.FireWizard:
                wizardPrefabID = BoltPrefabs.FireWizard;
                spellbook = transform.FindChild("Spellbook-FireWizard").GetComponent<Spellbook>();

                break;

            case WizardType.ArcaneWizard:
                wizardPrefabID = BoltPrefabs.ArcaneWizard;
                spellbook = transform.FindChild("Spellbook-ArcaneWizard").GetComponent<Spellbook>();

                break;

            case WizardType.PlagueWizard:
                wizardPrefabID = BoltPrefabs.PlagueWizard;
                spellbook = transform.FindChild("Spellbook-PlagueWizard").GetComponent<Spellbook>();

                break;

            case WizardType.ShrekWizard:
                wizardPrefabID = BoltPrefabs.ShrekWizard;
                spellbook = transform.FindChild("Spellbook-ShrekWizard").GetComponent<Spellbook>();

                break;
        }

        BoltEntity wizard = BoltNetwork.Instantiate(wizardPrefabID, userStatsToken, spawnLocation, Quaternion.identity);

        // attach spellbook spells

        Spellbook wizardSpellbook = wizard.GetComponent<Spellbook>();

        wizardSpellbook.SetSpell(0, spellbook.GetSpell(0));
        wizardSpellbook.SetSpell(1, spellbook.GetSpell(1));
        wizardSpellbook.SetSpell(2, spellbook.GetSpell(2));
        wizardSpellbook.SetSpell(3, spellbook.GetSpell(3));

        return wizard;
         
    }
    */

    public void EnableObject(GameObject toEnable)
    {
        toEnable.SetActive(true);
    }

    

    /// <summary>
    /// start game session
    /// NOTE: ONLY CALLED FROM SERVER
    /// </summary>
    public void StartGame()
    {
        // decide player colors and send to all connected clients
        int count = 0;

        foreach(CredentialToken player in ServerManager.Instance.ConnectedUsers)
        {
            SerializedColor playerColor = new SerializedColor(possibleColors[count]);

            PlayerColors[player] = playerColor;

            count++;
        }

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        bf.Serialize(ms, PlayerColors);

        SetColors setColors = SetColors.Create(Bolt.GlobalTargets.Others);

        setColors.BinaryData = ms.ToArray();

        setColors.Send();

        // = SendConnectedClients.Create(connection);

        //sendClients.BinaryData = ms.ToArray();

        //sendClients.Send();

        SceneChangeToken token = new SceneChangeToken();

        token.Reason = "StartGame";
        token.SceneTo = MainGameScene;
        token.SceneFrom = Application.loadedLevelName;

        Debug.Log("Starting game from gamemanager...");

        //Instantiate(EndGameManagerPrefab);
        //Instantiate(GameTypeManagerPrefab);

        BoltNetwork.LoadScene("Game", token);



        
    }

    /// <summary>
    /// NOTE: ONLY CALLED ON SERVER
    /// </summary>
    public void StartSetup()
    {
        StartCoroutine(startSetup());
    }

    private IEnumerator startSetup()
    {
        int playerCount = ServerManager.Instance.ConnectedUsers.Count;

        // give each player a turn to pick a region until all regions are picked
        for (int i = 0; i < MapManager.Instance.MapRegions.Keys.Count; i++ )
        {
            CredentialToken player = ServerManager.Instance.ConnectedUsers[i % playerCount];

            BoltConnection nextPlayer = ServerManager.Instance.Connections[player];

            PickRegion evnt = PickRegion.Create(nextPlayer);
            evnt.Send();

            pickingRegion = true;

            yield return new WaitUntil(() => pickingRegion == false);
        }

        EndSetup es = EndSetup.Create(Bolt.GlobalTargets.AllClients);

        es.Send();

        StartTurns();

        yield return null;
    }

    public void PickRegionOver()
    {
        pickingRegion = false;
    }

    public void TakeTurnOver()
    {
        takingTurn = false;
    }

    /// <summary>
    /// Starts the main game turn loop
    /// </summary>
    public void StartTurns()
    {
        StartCoroutine(startTurns());

    }

    private IEnumerator startTurns()
    {
        int playerCount = 0;
        int players = ServerManager.Instance.ConnectedUsers.Count;

        // go til end
        while(true)
        {
            // assign turn

            // turn order is currently in order of who connected
            CredentialToken player = ServerManager.Instance.ConnectedUsers[playerCount % players];

            BoltConnection nextPlayer = ServerManager.Instance.Connections[player];

            TakeTurn tt = TakeTurn.Create(nextPlayer);

            tt.Send();

            takingTurn = true;

            yield return new WaitUntil(() => takingTurn == false);


            // resolve turn

        }

        yield return null;
    }


}

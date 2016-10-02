using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [Serializable]
    public class PlayerIDDatabase : SerializedDictionary<int, BoltConnection> 
    { 

    }

    [SerializeField]
    public PlayerIDDatabase PlayerDatabase = new PlayerIDDatabase();
    
    private bool netInit = false;

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
        SceneChangeToken token = new SceneChangeToken();

        token.Reason = "StartGame";
        token.SceneTo = MainGameScene;
        token.SceneFrom = Application.loadedLevelName;

        Debug.Log("Starting game from gamemanager...");

        //Instantiate(EndGameManagerPrefab);
        //Instantiate(GameTypeManagerPrefab);

        BoltNetwork.LoadScene("Game", token);

        
    }

    public void StartSetup()
    {
        StartCoroutine(startSetup());
    }

    private IEnumerator startSetup()
    {
        yield return null;
    }


}

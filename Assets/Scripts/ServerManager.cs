using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

[Serializable]
public class UserStats
{
    public string DisplayName;

    public int PlayerID;

    public UserStats()
    {

    }
}

public class ServerManager : MonoBehaviour
{
    private static ServerManager _instance;

    /// <summary>
    /// users currently connected to this session
    /// </summary>
    public List<CredentialToken> ConnectedUsers = new List<CredentialToken>();

    /// <summary>
    /// users who were, at some point, connected to this session
    /// </summary>
    public List<CredentialToken> SessionUsers = new List<CredentialToken>();

    [Serializable]
    public class UserStatsDatabase : SerializedDictionary<string, UserStatsToken> { }

    /// <summary>
    /// record of user stats for this session
    /// </summary>
    [SerializeField]
    public UserStatsDatabase UserStats = new UserStatsDatabase();

    /// <summary>
    /// Maps credential tokens to the connection that is attached to this token
    /// </summary>
    [Serializable]
    public class ConnectionDatabase : SerializedDictionary<CredentialToken, BoltConnection>
    {

    }

    [SerializeField]
    public ConnectionDatabase Connections = new ConnectionDatabase();

    /// <summary>
    /// Maps connection to its attached credential token
    /// </summary>
    [Serializable]
    public class CredentialDatabase : SerializedDictionary<BoltConnection, CredentialToken>
    {

    }

    [SerializeField]
    public CredentialDatabase Credentials = new CredentialDatabase();


    public static ServerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ServerManager>();
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    void Update()
    {
        /*
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            if(BoltNetwork.isServer == true)
            {
                RegisterKill(ConnectedUsers[0].UserName);
                RegisterKill(ConnectedUsers[0].UserName);
                RegisterKill(ConnectedUsers[0].UserName);
                RegisterKill(ConnectedUsers[0].UserName);

                RegisterKill(ConnectedUsers[1].UserName);
                RegisterKill(ConnectedUsers[1].UserName);
                RegisterKill(ConnectedUsers[1].UserName);

            }
        }
        */
    }

    /// <summary>
    /// add user to this game session
    /// </summary>
    /// <param name="user"></param>
    public void AddToSession(CredentialToken user)
    {
        if (WasUserInSession(user.UserName) == true)
        {
            // this user was previously connected to this session, reconnect them and re-attach shit

            ConnectedUsers.Add(user);

            // also, only ONE user in the session may have this username 
            // so we need to remove that user and replace with this one, if anything is different

            CredentialToken previousUser = SessionUsers.Find(x => x.UserName == user.UserName);

            if (user != previousUser)
            {
                SessionUsers.Remove(previousUser);
                SessionUsers.Add(user);
            }

            return;
        }

        if (ConnectedUsers.Contains(user) == false)
        {
            Debug.Log("adding to lobby");


            ConnectedUsers.Add(user);
            SessionUsers.Add(user);

        }
    }

    /// <summary>
    /// remove user from game session
    /// </summary>
    /// <param name="user"></param>
    public void RemoveFromSession(CredentialToken user)
    {
        if (ConnectedUsers.Contains(user) == true)
        {
            Debug.Log("removing from lobby");

            ConnectedUsers.Remove(user);
        }

        //Messenger.Broadcast("UpdateLobby");
    }

    /// <summary>
    /// get token of connected user by searching their IP
    /// </summary>
    /// <param name="IP"></param>
    /// <returns></returns>
    public CredentialToken GetConnectedTokenByIP(string IP)
    {
        return ConnectedUsers.Find(x => x.IP == IP);
    }

    /// <summary>
    /// was the user ever connected to this session?
    /// </summary>
    /// <param name="displayName"></param>
    /// <returns></returns>
    public bool WasUserInSession(string displayName)
    {
        return SessionUsers.Exists(x => x.UserName == displayName);
    }

    /// <summary>
    /// is the user currently connected to this session?
    /// </summary>
    /// <param name="displayName"></param>
    /// <returns></returns>
    public bool IsUserConnected(string displayName)
    {
        return ConnectedUsers.Exists(x => x.UserName == displayName);
    }

    


    /// <summary>
    /// sends event to all other clients to update user stats
    /// </summary>
    /// <param name="displayName"></param>
    /// <param name="stats"></param>
    private void updateUserStats(string displayName, UserStatsToken stats)
    {
        UpdateUserStats evnt = UpdateUserStats.Create(Bolt.GlobalTargets.Others);

        evnt.UserName = displayName;
        evnt.UserToken = UserStats[displayName];

        evnt.Send();
    }

    /// <summary>
    /// Update client GUI user stats (ONLY CALLED BY CLIENTS, NOT SERVER)
    /// </summary>
    public void UpdateClientUserStats(string displayName, UserStatsToken stats)
    {
        UserStats[displayName] = stats;
    }

    /// <summary>
    /// start server for this game session
    /// </summary>
    public void StartServer()
    {
        //Instantiate(GameManager.Instance.ServerManagerPrefab);
       // BoltLauncher.StartServer(UdpKit.UdpEndPoint.Parse("127.0.0.1:27000"));
        //BoltLauncher.StartServer(UdpKit.UdpEndPoint.Any);
        InitGameSession();
        //UdpKit.UdpEndPoint server = new UdpKit.UdpEndPoint(UdpKit.UdpIPv4Address.Localhost, 27000);
        //BoltLauncher.StartServer(server);
        BoltLauncher.StartServer(27000);
        //ServerManager.Instance.InitGameSession();
        //BoltNetwork.LoadScene("Tutorial1");


        //BoltLauncher.StartClient();
    }

    /// <summary>
    /// initialize this game session
    /// </summary>
    public void InitGameSession()
    {
        Debug.Log("init game");
        ConnectedUsers.Clear();

        SessionUsers.Clear();

        UserStats.Clear();

    }

    /// <summary>
    /// initializes user stats when game starts
    /// </summary>
    public void InitUserStats()
    {
        UserStats.Clear();

        int connectedUsers = 0;

        foreach (CredentialToken user in ConnectedUsers)
        {
            UserStatsToken testToken = new UserStatsToken();
            testToken.DisplayName = user.DisplayName;
            testToken.PlayerID = connectedUsers++;

            UserStats.Add(user.DisplayName, testToken);
        }
    }

    public void GameEnd()
    {
        // end game time

        // disconnect clients here
        foreach (var conn in BoltNetwork.connections)
        {
            // create EndGameToken
            EndGameToken endGametoken = new EndGameToken();
            // give it reason: game end
            endGametoken.EndGameReason = "Dis game done, son";

            // figure out who won the game, send that result in a parseable format
            endGametoken.Won = true;
            //conn.DisconnectToken = endGametoken;
            conn.Disconnect(endGametoken);


        }

        // set core gui manager end game state; also, did the server player win the game?
        CoreGUIManager.Instance.SetEndGame(true);

        GameGUIManager.Instance.BoltUnloadGameScene();

        BoltLauncher.Shutdown();

        BoltNetwork.LoadScene(GameManager.Instance.MenuSceneName);
    }




}

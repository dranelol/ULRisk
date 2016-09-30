using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{
    public override void BoltStartDone()
    {
        BoltNetwork.RegisterTokenClass<CredentialToken>();
        BoltNetwork.RegisterTokenClass<SceneChangeToken>();
        BoltNetwork.RegisterTokenClass<UserStatsToken>();
        BoltNetwork.RegisterTokenClass<EndGameToken>();

        if (BoltNetwork.isServer == true)
        {
            BoltNetwork.SetHostInfo("Here", null);
        }

        if (BoltNetwork.isServer == false)
        {
            //BoltNetwork.Connect(UdpKit.UdpEndPoint.Parse(ClientManager.Instance.ConnectIP));
            //BoltNetwork.Connect(UdpKit.UdpEndPoint.Parse(ClientManager.Instance.ConnectIP + ":27000"), ClientManager.Instance.Credentials);
            //UdpKit.UdpEndPoint client = new UdpKit.UdpEndPoint(UdpKit.UdpIPv4Address.Localhost, 27000);
            Debug.Log("connecting...");
            BoltNetwork.Connect(UdpKit.UdpEndPoint.Parse("127.0.0.1:27000"));

            //BoltNetwork.Connect(client, ClientManager.Instance.Credentials);


        }
    }

    public override void ZeusConnected(UdpKit.UdpEndPoint endpoint)
    {
        Bolt.Zeus.RequestSessionList();
    }

    public override void SceneLoadLocalBegin(string scene, Bolt.IProtocolToken token)
    {
        SceneChangeToken sceneChange = (SceneChangeToken)token;

        if (sceneChange != null)
        {
            Debug.Log("loading from scene: " + sceneChange.SceneFrom);
            Debug.Log("loading to scene: " + sceneChange.SceneTo);
            Debug.Log("reason: " + sceneChange.Reason);

        }
    }

    /// <summary>
    /// Called when a local scene change finishes.
    /// </summary>
    /// <param name="map"></param>
    /// <param name="token"></param>
    public override void SceneLoadLocalDone(string map, Bolt.IProtocolToken token)
    {
        SceneChangeToken sceneChange = (SceneChangeToken)token;

        if (sceneChange != null)
        {
            Debug.Log("loading from scene: " + sceneChange.SceneFrom);
            Debug.Log("loading to scene: " + sceneChange.SceneTo);
            Debug.Log("reason: " + sceneChange.Reason);

            if (sceneChange.Reason == "StartGame")
            {
                if (BoltNetwork.isClient)
                {
                    GameplayStartedEvent evnt = GameplayStartedEvent.Create(Bolt.GlobalTargets.OnlyServer);
                    CredentialToken credentials = ClientManager.Instance.Credentials;
                    evnt.CredentialsToken = credentials;
                    evnt.Send();
                }

                else
                {
                    // we're the server
                    /*
                    BoltEntity wizard = GameManager.Instance.SpawnWizard(ClientManager.Instance.Credentials);
                    wizard.GetComponent<WizardController>().Init();
                    wizard.TakeControl();

                    BoltEntity entropyManager = GameManager.Instance.SpawnEntropyManager();

                    entropyManager.TakeControl();
                     */

                    // enable the start of setup for the game
                    GameManager.Instance.StartSetup();
                }

                ServerManager.Instance.InitUserStats();

                //GameTypeManager.Instance.RoundStart();

                //StartMenuGUIManager.Instance.FSM.Transition("Init");

                GameGUIManager.Instance.BoltLoadGameScene();
            }
        }


    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {

    }

    List<string> logMessages = new List<string>();

    #region OnEvents

    public override void OnEvent(LogEvent evnt)
    {
        logMessages.Insert(0, evnt.message);
    }

    public override void OnEvent(UserJoinedLobby evnt)
    {

        Debug.Log("user joined lobby, raised from: " + ((CredentialToken)evnt.UserToken).DisplayName);

        ServerManager.Instance.AddToSession((CredentialToken)evnt.UserToken);

        //Debug.Log(evnt.RaisedBy.ToString());
    }

    public override void OnEvent(UserDisconnectedLobby evnt)
    {

        Debug.Log("user left lobby, raised from: " + ((CredentialToken)evnt.UserToken).DisplayName);

        ServerManager.Instance.RemoveFromSession((CredentialToken)evnt.UserToken);
    }

    public override void OnEvent(EndGame evnt)
    {

    }

    /// <summary>
    /// Sent by the server
    /// Received by a newly-connected client
    /// Server sends list of connected clients to the server to populate server lobby GUI
    /// </summary>
    /// <param name="evnt"></param>
    public override void OnEvent(SendConnectedClients evnt)
    {
        MemoryStream ms = new MemoryStream();
        BinaryFormatter bf = new BinaryFormatter();
        ms.Write(evnt.BinaryData, 0, evnt.BinaryData.Length);
        ms.Seek(0, SeekOrigin.Begin);

        List<CredentialToken> connectedCredentials = (List<CredentialToken>)bf.Deserialize(ms);

        foreach (CredentialToken token in connectedCredentials)
        {
            //Messenger.Broadcast("UserAddedToLobby", token.DisplayName);
            ServerManager.Instance.AddToSession(token);
        }
    }


   

    /// <summary>
    /// Sent by client starting their game
    /// Received by Server
    /// Spawns everything needed to start game by client, gives control to the event sender 
    /// </summary>
    /// <param name="evnt"></param>
    public override void OnEvent(GameplayStartedEvent evnt)
    {
        if (BoltNetwork.isServer)
        {
            //BoltEntity wizard = GameManager.Instance.SpawnWizard(evnt.CredentialsToken as CredentialToken);
            //Debug.Log(evnt.RaisedBy.ConnectToken);
            //wizard.AssignControl(evnt.RaisedBy);

            //GameManager.Instance.WizardSpawned(newWizard);
        }
    }

    



    /// <summary>
    /// Sent by: Server
    /// Received by: All other clients
    /// Update user stats for the client GUI
    /// </summary>
    /// <param name="evnt"></param>
    public override void OnEvent(UpdateUserStats evnt)
    {
        ServerManager.Instance.UpdateClientUserStats(evnt.UserName, (UserStatsToken)evnt.UserToken);
    }

    
    #endregion
}

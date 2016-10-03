using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Bolt;


[BoltGlobalBehaviour(BoltNetworkModes.Host)]
public class ServerCallbacks : GlobalEventListener
{

    /// <summary>
    /// called when someone connects to server
    /// </summary>
    /// <param name="connection"></param>
    public override void Connected(BoltConnection connection)
    {
        IProtocolToken acceptToken = connection.AcceptToken;
        IProtocolToken connectToken = connection.ConnectToken;

        var log = LogEvent.Create();
        //log.Message = string.Format("{0} connected", connection.RemoteEndPoint);

        // connected to server
        CredentialToken token = (CredentialToken)connectToken;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != GameManager.Instance.MenuSceneName)
        {
            Debug.Log("client tried to connect during gameplay: " + token.UserName);
            return;
        }

        Debug.Log("token connected: " + token.UserName);

        log.message += "connected: " + token.UserName;
        log.Send();


        // maintain credential/connection database
        ServerManager.Instance.Players.Add(token, connection);
        ServerManager.Instance.AddToSession(token);

        // grab all connections to the server, get their credentials
        // serialize that list, and send it as an event
        List<CredentialToken> connectedCredentials = new List<CredentialToken>();

        foreach (BoltConnection conn in BoltNetwork.connections)
        {
            if (conn != connection)
            {
                connectedCredentials.Add((CredentialToken)conn.ConnectToken);
                Debug.Log(((CredentialToken)conn.ConnectToken).DisplayName);

                // tell all clients except for this one that we're joining
                var userJoined = UserJoinedLobby.Create(conn);

                userJoined.UserToken = token;

                userJoined.Send();
            }
        }

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        bf.Serialize(ms, connectedCredentials);

        // send the list of connected clients to the newly-connected client
        SendConnectedClients sendClients = SendConnectedClients.Create(connection);

        sendClients.BinaryData = ms.ToArray();

        sendClients.Send();
    }

    public override void Disconnected(BoltConnection connection)
    {
        var log = LogEvent.Create();
        log.message = string.Format("{0} disconnected", connection.RemoteEndPoint.Address.ToString());
        log.Send();

        Debug.Log("token disconnected: " + connection.RemoteEndPoint.Address.ToString());

        // get token connected with connection's ip

        //CredentialToken disconnectToken = ServerManager.Instance.GetConnectedTokenByIP(connection.RemoteEndPoint.Address.ToString());
        CredentialToken disconnectToken = (CredentialToken)connection.ConnectToken;


        // tell all clients that a user is leaving the lobby so that they may update their GUIs and such

        var userDisconnected = UserDisconnectedLobby.Create();

        userDisconnected.UserToken = disconnectToken;
        userDisconnected.Send();
    }

}

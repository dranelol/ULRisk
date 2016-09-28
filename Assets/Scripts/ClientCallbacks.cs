using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


[BoltGlobalBehaviour(BoltNetworkModes.Client)]
public class ClientCallbacks : GlobalEventListener
{
    /// <summary>
    /// called when this client disconnects from server
    /// </summary>
    /// <param name="connection"></param>
    public override void Disconnected(BoltConnection connection)
    {
        Debug.Log("disconnected!");

        Debug.Log("its endgame time!");

        EndGameToken token = (EndGameToken)connection.DisconnectToken;


        // notify the start gui to transition to start_menu

        // set core gui manager end game state; also, did this client win the game?
        CoreGUIManager.Instance.SetEndGame(true);
        // load menu scene
        //BoltLauncher.Shutdown();
        GameGUIManager.Instance.BoltUnloadGameScene();

        Application.LoadLevel(GameManager.Instance.MenuSceneName);
        //BoltNetwork.LoadScene("menu");
    }
}

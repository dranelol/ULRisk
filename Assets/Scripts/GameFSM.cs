using UnityEngine;
using System.Collections;

public enum GameStates
{
    SetupIdle,
    SetupPickRegion,
    BeginTurn,
    PreReinforce,
    Action,
    Resolve,
    EndTurn,
    PostReinforce,
    EndGame
}


public class GameFSM : StateMachine
{

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}

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
    void Awake()
    {
        SetupMachine(GameStates.SetupIdle);

        AddTransition(GameStates.SetupIdle, GameStates.SetupPickRegion);
        AddTransition(GameStates.SetupPickRegion, GameStates.SetupIdle);

        AddTransition(GameStates.SetupIdle, GameStates.BeginTurn);
        AddTransition(GameStates.SetupPickRegion, GameStates.BeginTurn);


    }

    #region mono methods
    void Start ()
    {
	    
	}
	

	void Update ()
    {

    }

    #endregion

    #region public methods

    #endregion

    #region state methods

    #region setup idle methods
    IEnumerator SetupIdle_EnterState()
    {
        yield return null;
    }
    void SetupIdle_Update()
    {

    }

    IEnumerator SetupIdle_ExitState()
    {
        yield return null;
    }
    #endregion

    #region setup pick region methods
    IEnumerator SetupPickRegion_EnterState()
    {
        yield return null;
    }
    void SetupPickRegion_Update()
    {

    }

    IEnumerator SetupPickRegion_ExitState()
    {
        yield return null;
    }
    #endregion

    #region begin turn methods
    IEnumerator BeginTurn_EnterState()
    {
        yield return null;
    }
    void BeginTurn_Update()
    {

    }

    IEnumerator BeginTurn_ExitState()
    {
        yield return null;
    }
    #endregion

    #endregion
}

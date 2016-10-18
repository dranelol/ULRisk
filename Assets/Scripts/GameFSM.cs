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

    public void StartGame()
    {
        StartMachine(GameStates.SetupIdle);
    }

    public void PickRegion()
    {
        transition(GameStates.SetupPickRegion);
    }

    public void DonePickRegion()
    {
        transition(GameStates.SetupIdle);
    }
    #endregion

    #region state methods

    #region setup idle methods
    IEnumerator SetupIdle_EnterState()
    {
        UIDisableInput.Disable();

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
        UIDisableInput.Enable(true);

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

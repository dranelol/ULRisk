// A State Machine class that helps building FSMs using Unity Monobehavior functions
// Author: Matt Wallace
// Last Edited: 10.20.2014

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Reflection;

public abstract class StateMachine : MonoBehaviour
{
    #region fields

    public bool Debugging = false;

    /// <summary>
    /// Returns whether or not the current state's next state is itself
    /// </summary>
    private bool reflexiveTransition;

    private string FSMName;

    private Type stateType = null;

    private Enum currentState = null;

    public Enum CurrentState
    {
        get
        {
            return currentState;
        }
    }

    private Enum previousState = null;

    public Enum PreviousState
    {
        get
        {
            return previousState;
        }
    }

    private Enum previousPreviousState = null;

    public Enum PreviousPreviousState 
    {
        get
        {
            return previousPreviousState;
        }
    }

    private Dictionary<Enum, HashSet<Enum>> Transitions = null;

    private Dictionary<Pair<Enum, Enum>, Delegate> Triggers = null;

    private Dictionary<Pair<Enum, Enum>, Delegate> Handlers = null;

    #endregion

    #region constructors
    #endregion 

    #region methods

    #region static default methods
    static void DefaultFunction()
    {

    }

    static void DefaultCollider(Collider other)
    {
    }

    static void DefaultCollision(Collision other)
    {
    }

    static IEnumerator DefaultCoroutine()
    {
        yield break;
    }

    static bool DefaultTrigger()
    {
        return true;
    }

    #endregion

    #region delegate functions

    public Action DoUpdate = DefaultFunction;
    public Action DoLateUpdate = DefaultFunction;
    public Action DoFixedUpdate = DefaultFunction;

    public Action Handler = DefaultFunction;
    public Func<bool> Trigger = DefaultTrigger;

    public Action<Collider> DoOnTriggerEnter = DefaultCollider;
    public Action<Collider> DoOnTriggerStay = DefaultCollider;
    public Action<Collider> DoOnTriggerExit = DefaultCollider;
    public Action<Collision> DoOnCollisionEnter = DefaultCollision;
    public Action<Collision> DoOnCollisionStay = DefaultCollision;
    public Action<Collision> DoOnCollisionExit = DefaultCollision;

    public Action DoOnMouseEnter = DefaultFunction;
    public Action DoOnMouseUp = DefaultFunction;
    public Action DoOnMouseDown = DefaultFunction;
    public Action DoOnMouseOver = DefaultFunction;
    public Action DoOnMouseExit = DefaultFunction;
    public Action DoOnMouseDrag = DefaultFunction;
    public Action DoOnGUI = DefaultFunction;

    public Func<IEnumerator> EnterState = DefaultCoroutine;
    public Func<IEnumerator> ExitState = DefaultCoroutine;
    public Func<IEnumerator> StayState = DefaultCoroutine;

    #endregion

    /// <summary>
    /// Sets up the state machine by giving it an example of a state it describes
    /// </summary>
    /// <param name="typeSet">Example of type of states handled by this FSM</param>
    public void SetupMachine(Enum typeSet)
    {
        Transitions = new Dictionary<Enum, HashSet<Enum>>();
        stateType = typeSet.GetType();
        var stateTypes = Enum.GetValues(stateType);

        foreach (Enum state in stateTypes)
        {
            Transitions.Add(state, new HashSet<Enum>());
        }
    }

    /// <summary>
    /// Starts the FSM. Call this after setting up the entire FSM and transitions
    /// </summary>
    /// <param name="startState">The state from which the FSM is started</param>
    public void StartMachine(Enum startState)
    {
        if (currentState == null)
        {
            currentState = startState;
        }
        FSMName = this.GetType().Name;
        ConfigureCurrentState();
        
    }

    /// <summary>
    /// Transition to the next state given by "nextState"
    /// </summary>
    /// <param name="nextState"></param>
    protected void transition(Enum nextState)
    {
        if (Debugging == true)
        {
            Debug.Log("Transition from " + currentState.ToString() + " to " + nextState.ToString());
        }
        if (currentState == null)
        {
            throw new NullReferenceException("null current state");
        }

        //if the hashset of transitions contains a valid transition to next state
        

        if (Transitions[currentState].Contains(nextState))
        {
            // 0. set/verify all needed values
            // 1. need to call current state's exit
            // 2. set up new state functions
            // 3. need to call new state enter
            previousPreviousState = previousState;
            previousState = currentState;
            currentState = nextState;

            ConfigureCurrentState();

        }

        // a transition from current to next doesnt exist
        else
        {
            throw new NullReferenceException("no valid transition from: "
                + currentState.ToString()
                + " to next state: "
                + nextState.ToString()
                + " on object " + gameObject.name
                + " of fsm " + FSMName
                );
        }

    }
    /// <summary>
    /// Returns whether or not there is a valid transition from "fromState" to "toState"
    /// </summary>
    /// <param name="fromState"></param>
    /// <param name="toState"></param>
    /// <returns></returns>
    public bool IsValidTransition(Enum fromState, Enum toState)
    {
        return Transitions[fromState].Contains(toState);
    }

    /// <summary>
    /// magic.
    /// </summary>
    private void ConfigureCurrentState()
    {
        // Debug.Log("previous state: " + previousState);
        // Debug.Log("current state: " + currentState);

        if (previousState != null)
        {
            if (previousState.ToString() == currentState.ToString())
            {

                if (Debugging == true)
                {
                    Debug.Log("calling stay state for state: " + currentState.ToString() + " of fsm: " + FSMName);
                }

                StartCoroutine(StayState());
                // dont need to do anything else, we're in a reflexive transition
                return;
            }


        }

        // call exit state function for old current state
        if (previousState != null && ExitState != null)
        {
            if (Debugging == true)
            {
                Debug.Log("calling exit state for state: " + previousState.ToString() + " of fsm: " + FSMName);
            }
            StartCoroutine(ExitState());
        }

        // update all state function delegates

        DoUpdate = ConfigureDelegate<Action>("Update", DefaultFunction);
        DoOnGUI = ConfigureDelegate<Action>("OnGUI", DefaultFunction);
        DoLateUpdate = ConfigureDelegate<Action>("LateUpdate", DefaultFunction);
        DoFixedUpdate = ConfigureDelegate<Action>("FixedUpdate", DefaultFunction);
        DoOnMouseUp = ConfigureDelegate<Action>("OnMouseUp", DefaultFunction);
        DoOnMouseDown = ConfigureDelegate<Action>("OnMouseDown", DefaultFunction);
        DoOnMouseEnter = ConfigureDelegate<Action>("OnMouseEnter", DefaultFunction);
        DoOnMouseExit = ConfigureDelegate<Action>("OnMouseExit", DefaultFunction);
        DoOnMouseDrag = ConfigureDelegate<Action>("OnMouseDrag", DefaultFunction);
        DoOnMouseOver = ConfigureDelegate<Action>("OnMouseOver", DefaultFunction);
        DoOnTriggerEnter = ConfigureDelegate<Action<Collider>>("OnTriggerEnter", DefaultCollider);
        DoOnTriggerExit = ConfigureDelegate<Action<Collider>>("OnTriggerExit", DefaultCollider);
        DoOnTriggerStay = ConfigureDelegate<Action<Collider>>("OnTriggerEnter", DefaultCollider);
        DoOnCollisionEnter = ConfigureDelegate<Action<Collision>>("OnCollisionEnter", DefaultCollision);
        DoOnCollisionExit = ConfigureDelegate<Action<Collision>>("OnCollisionExit", DefaultCollision);
        DoOnCollisionStay = ConfigureDelegate<Action<Collision>>("OnCollisionStay", DefaultCollision);
        EnterState = ConfigureDelegate<Func<IEnumerator>>("EnterState", DefaultCoroutine);
        ExitState = ConfigureDelegate<Func<IEnumerator>>("ExitState", DefaultCoroutine);
        StayState = ConfigureDelegate<Func<IEnumerator>>("StayState", DefaultCoroutine);

        if (previousState != null && currentState != null)
        {
            Handler = ConfigureHandler<Action>(previousState.ToString(), currentState.ToString(), DefaultFunction);
            Handler();
        }

        if (previousState != null && currentState != null)
        {
            Trigger = ConfigureTrigger<Func<bool>>(previousState.ToString(), currentState.ToString(), DefaultTrigger);
            
        }

        
        // call state transition handler
        /*
        Pair<Enum, Enum> transition = new Pair<Enum, Enum>(previousState, currentState);

        if (Handlers.ContainsKey(transition) == true)
        {
            
        }
        */

        // call enter state function for new current state
        if (EnterState != null)
        {
            if (Debugging == true)
            {
                Debug.Log("calling enter state for state: " + currentState.ToString() + " of fsm: " + FSMName);
            }

            StartCoroutine(EnterState());
        }

    }

    /// <summary>
    /// More magic.
    /// </summary>
    /// <typeparam name="T">Magic.</typeparam>
    /// <param name="methodRoot">More magic.</param>
    /// <param name="Default">Voodoo.</param>
    /// <returns></returns>
    private T ConfigureDelegate<T>(string methodRoot, T Default) where T : class
    {
        List<string> methodNames = new List<string>();

        BindingFlags methodTypes = System.Reflection.BindingFlags.Instance
                                 | System.Reflection.BindingFlags.Public 
                                 | System.Reflection.BindingFlags.NonPublic
                                 | System.Reflection.BindingFlags.InvokeMethod;

        string currentStatePure = currentState.ToString();
        string currentStateChangeCasing = currentStatePure;

        // switch casing of first letter to check for both types of methods
        if (char.IsUpper(currentStateChangeCasing, 0) == true)
        {
            currentStateChangeCasing = currentStatePure.First().ToString().ToLower() + currentStatePure.Substring(1);
        }

        else
        {
            currentStateChangeCasing = currentStatePure.First().ToString().ToUpper() + currentStatePure.Substring(1);
        }

        methodNames.Add(currentStatePure + "_" + methodRoot);
        methodNames.Add(currentStateChangeCasing + "_" + methodRoot);
        

        foreach (string methodName in methodNames)
        {
            
            MethodInfo mtd = GetType().GetMethod(methodName, methodTypes);


            if (mtd != null)
            {
                if (Debugging == true)
                {
                    Debug.Log("setting delegate for: " + currentState.ToString() + "_" + methodRoot);
                }

                return Delegate.CreateDelegate(typeof(T), this, mtd) as T;
            }
        }

        return Default;
    }

    private T ConfigureHandler<T>(string previous, string current, T Default) where T : class
    {
        List<string> methodNames = new List<string>();

        BindingFlags methodTypes = System.Reflection.BindingFlags.Instance
                                 | System.Reflection.BindingFlags.Public
                                 | System.Reflection.BindingFlags.NonPublic
                                 | System.Reflection.BindingFlags.InvokeMethod;

        string currentStatePure = currentState.ToString();
        string currentStateChangeCasing = currentStatePure;

        string previousStatePure = previousState.ToString();
        string previousStateChangeCasing = previousStatePure;

        // switch casing of first letter to check for both types of methods
        if (char.IsUpper(currentStateChangeCasing, 0) == true)
        {
            currentStateChangeCasing = currentStatePure.First().ToString().ToLower() + currentStatePure.Substring(1);
        }

        else
        {
            currentStateChangeCasing = currentStatePure.First().ToString().ToUpper() + currentStatePure.Substring(1);
        }

        if (char.IsUpper(previousStateChangeCasing, 0) == true)
        {
            previousStateChangeCasing = previousStatePure.First().ToString().ToLower() + previousStatePure.Substring(1);
        }

        else
        {
            previousStateChangeCasing = previousStatePure.First().ToString().ToUpper() + previousStatePure.Substring(1);
        }

        methodNames.Add(previousStatePure + "To" + currentStatePure + "_Handler");
        methodNames.Add(previousStatePure + "To" + currentStateChangeCasing + "_Handler");
        methodNames.Add(previousStateChangeCasing + "To" + currentStatePure + "_Handler");
        methodNames.Add(previousStateChangeCasing + "To" + currentStateChangeCasing + "_Handler");

        foreach (string methodName in methodNames)
        {
            if (Debugging == true)
            {
                Debug.Log("searching for handler: " + methodName);
            }
            MethodInfo mtd = GetType().GetMethod(methodName, methodTypes);

            if (mtd != null)
            {
                if (Debugging == true)
                {
                    Debug.Log("setting handler for: " + previousState.ToString() + " to " + currentState.ToString());
                }

                return Delegate.CreateDelegate(typeof(T), this, mtd) as T;
            }
        }

        return Default;
    }

    private T ConfigureTrigger<T>(string previous, string current, T Default) where T : class
    {
        List<string> methodNames = new List<string>();

        BindingFlags methodTypes = System.Reflection.BindingFlags.Instance
                                 | System.Reflection.BindingFlags.Public
                                 | System.Reflection.BindingFlags.NonPublic
                                 | System.Reflection.BindingFlags.InvokeMethod;

        string currentStatePure = currentState.ToString();
        string currentStateChangeCasing = currentStatePure;

        string previousStatePure = previousState.ToString();
        string previousStateChangeCasing = previousStatePure;

        // switch casing of first letter to check for both types of methods
        if (char.IsUpper(currentStateChangeCasing, 0) == true)
        {
            currentStateChangeCasing = currentStatePure.First().ToString().ToLower() + currentStatePure.Substring(1);
        }

        else
        {
            currentStateChangeCasing = currentStatePure.First().ToString().ToUpper() + currentStatePure.Substring(1);
        }

        if (char.IsUpper(previousStateChangeCasing, 0) == true)
        {
            previousStateChangeCasing = previousStatePure.First().ToString().ToLower() + previousStatePure.Substring(1);
        }

        else
        {
            previousStateChangeCasing = previousStatePure.First().ToString().ToUpper() + previousStatePure.Substring(1);
        }

        methodNames.Add(previousStatePure + "To" + currentStatePure + "_Trigger");
        methodNames.Add(previousStatePure + "To" + currentStateChangeCasing + "_Trigger");
        methodNames.Add(previousStateChangeCasing + "To" + currentStatePure + "_Trigger");
        methodNames.Add(previousStateChangeCasing + "To" + currentStateChangeCasing + "_Trigger");

        foreach (string methodName in methodNames)
        {
            if (Debugging == true)
            {
                Debug.Log("searching for trigger: " + methodName);
            }

            MethodInfo mtd = GetType().GetMethod(methodName, methodTypes);

            if (mtd != null)
            {
                if (Debugging == true)
                {
                    Debug.Log("setting handler for: " + previousState.ToString() + " to " + currentState.ToString());
                }

                return Delegate.CreateDelegate(typeof(T), this, mtd) as T;
            }
        }

        return Default;
    }
    /// <summary>
    /// Adds a transition from "state" to every state in statesList
    /// </summary>
    /// <param name="state">State to transition from</param>
    protected void AddAllTransitionsFrom(Enum state)
    {
        foreach (Enum transition in Transitions.Keys)
        {
            AddTransition(state, transition);
        }
    }

    /// <summary>
    /// Adds a transition from every state in statesList to "state"
    /// </summary>
    /// <param name="state">State to tranistion to</param>
    protected void AddAllTransitionsTo(Enum state)
    {
        foreach (Enum transition in Transitions.Keys)
        {
            AddTransition(transition, state);
        }
    }

    /// <summary>
    /// Adds a transition from "state" to every state in "transitions"
    /// </summary>
    /// <param name="state">State to transition from</param>
    /// <param name="transitions">States to transitions to</param>
    protected void AddTransitionsFrom(Enum state, HashSet<Enum> transitions)
    {
        if (transitions.Count > 0)
        {
            foreach (Enum transition in transitions)
            {
                AddTransition(state, transition);
            }
        }
    }

    /// <summary>
    /// Adds a transition from each state in "transitions" to "state"
    /// </summary>
    /// <param name="state">State to transitions to</param>
    /// <param name="transitions">States to transition from</param>
    protected void AddTransitionsTo(Enum state, HashSet<Enum> transitions)
    {
        if (transitions.Count > 0)
        {
            foreach (Enum transition in transitions)
            {
                AddTransition(transition, state);
            }
        }
    }

    /// <summary>
    /// adds transition from state "from" to state "to"
    /// </summary>
    /// <param name="fromState"></param>
    /// <param name="toState"></param>
    protected void AddTransition(Enum fromState, Enum toState)
    {
        // check state for statetype
        if (fromState.GetType() != stateType)
        {
            throw new Exception("Attempting to transition from an invalid state type");
        }
             
        else
        {
            // check transition for statetype
            if (toState.GetType() != stateType)
            {
                throw new Exception("Attempting to transition to an invalid state type");
            }

            else
            {
                // we can add the transition!
                Transitions[fromState].Add(toState);
            }
        }
    }

    #region game loop methods

    void Update()
    {
        bool triggerRet = Trigger();
        if (triggerRet == true)
        {
            if (Debugging == true)
            {
                Debug.Log("calling update for state: " + currentState.ToString() + " of fsm: " + FSMName);
            }

            DoUpdate();
        }
    }

    void LateUpdate()
    {
        if (Debugging == true)
        {
            Debug.Log("calling late update for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoLateUpdate();
    }

    void OnMouseEnter()
    {
        if (Debugging == true)
        {
            Debug.Log("calling on mouse enter for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnMouseEnter();
    }

    void OnMouseUp()
    {
        if (Debugging == true)
        {
            Debug.Log("calling on mouse up for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnMouseUp();
    }

    void OnMouseDown()
    {
        if (Debugging == true)
        {
            Debug.Log("calling on mouse down for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnMouseDown();
    }

    void OnMouseExit()
    {
        if (Debugging == true)
        {
            Debug.Log("calling on mouse exit for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnMouseExit();
    }

    void OnMouseDrag()
    {
        if (Debugging == true)
        {
            Debug.Log("calling on mouse drag for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnMouseDrag();
    }

    void FixedUpdate()
    {
        if (Debugging == true)
        {
            Debug.Log("calling fixed update for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoFixedUpdate();
    }
    void OnTriggerEnter(Collider other)
    {
        if (Debugging == true)
        {
            Debug.Log("calling on trigger enter for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnTriggerEnter(other);
    }
    void OnTriggerExit(Collider other)
    {
        if (Debugging == true)
        {
            Debug.Log("calling on trigger exit for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnTriggerExit(other);
    }
    void OnTriggerStay(Collider other)
    {
        if (Debugging == true)
        {
            Debug.Log("calling on trigger stay for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnTriggerStay(other);
    }
    void OnCollisionEnter(Collision other)
    {
        if (Debugging == true)
        {
            Debug.Log("calling on collision enter for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnCollisionEnter(other);
    }
    void OnCollisionExit(Collision other)
    {
        if (Debugging == true)
        {
            Debug.Log("calling on collision exit for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnCollisionExit(other);
    }
    void OnCollisionStay(Collision other)
    {
        if (Debugging == true)
        {
            Debug.Log("calling on collision stay for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnCollisionStay(other);
    }
    void OnGUI()
    {
        if (Debugging == true)
        {
            Debug.Log("calling on gui for state: " + currentState.ToString() + " of fsm: " + FSMName);
        }
        DoOnGUI();
    }

    #endregion

    #endregion

}
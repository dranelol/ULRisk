// Simple C# / Unity Messaging System
// Author: Matt Wallace
// Last Updated: 3.13.14
// Adapted from:
// Rod Hyde's "CSharpMessenger"
// Magnus Wolffelt's "CSharpMessenger Extended"
// Ilya Suzdalnitski's "Advanced C# Messenger"

using System;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;


#region Example Usage: Generic

// Function to be called when listener hears a message
/*
void OnPropCollected( PropType propType ) 
{
    if (propType == PropType.Life)
    {
        livesAmount++;
    }
}

// To add a listener of type PropType:
void Start() 
{
	Messenger.AddListener< PropType >( "prop collected", OnPropCollected );
}

// To remove a listener of type Proptype:
Messenger.RemoveListener< PropType > ( "prop collected", OnPropCollected );

public void OnTriggerEnter(Collider _collider) 
{
    // To broadcast a message of type PropType:
	Messenger.Broadcast< PropType > ( "prop collected", _collider.gameObject.GetComponent<Prop>().propType );
}

*/

#endregion

#region Example Usage: Specific
/*
// To add a listener subscribed to the GameManager gameobject:
Messenger.AddListenerSpecific(GameManager.Instance.gameObject, "DoStuff", DoAction);

// To broadcast a message from within GameManager to all objects subscribed to the GameManager:
Messenger.BroadcastSpecific(gameObject, "DoStuff");
 
// Function to be called when listener hears a message
void DoAction()
{
    Debug.Log("worked mofugga");
} 
 
*/
#endregion
//public delegate void Callback(params object[] args);

// Callback functions; one for each function type
public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);
public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);
/*
public delegate void CallbackSpecific<T>();
public delegate void CallbackSpecific<T,U>(U arg1);
public delegate void CallbackSpecific<T,U,V>(U arg1, V arg2);
public delegate void CallbackSpecific<T,U,V,W>(U arg1, V arg2, W arg3);
*/

public class BroadcastException : Exception
{
    public BroadcastException(string msg)
        : base(msg)
    {
    }
}

public class ListenerException : Exception
{
    public ListenerException(string msg)
        : base(msg)
    {
    }
}

internal class Messenger : MonoBehaviour
{
    // instantiates a messengerhelper object as a singleton
    static private MessengerHelper messengerHelper = (new GameObject("MessengerHelper")).AddComponent<MessengerHelper>();

    static public Dictionary<string, Delegate> events = new Dictionary<string, Delegate>();

    static public Dictionary<Pair<string, GameObject>, Delegate> eventsSpecific = new Dictionary<Pair<string, GameObject>, Delegate>();

    static public List<string> permanentMessages = new List<string>();
    
    /// <summary>
    /// adds a message to the list of permanent messages
    /// </summary>
    /// <param name="eventName"></param>
    static public void MakePermanent(string eventName)
    {
        permanentMessages.Add(eventName);
    }

    /// <summary>
    /// cleans up all messages
    /// </summary>
    static public void Cleanup()
    {
        List<string> messagesToRemove = new List<string>();

        //generic message cleanup
        foreach (KeyValuePair<string, Delegate> pair in events)
        {
            bool wasFound = false;

            foreach (string message in permanentMessages)
            {
                if (pair.Key == message)
                {
                    wasFound = true;
                    break;
                }
            }

            if (!wasFound)
                messagesToRemove.Add(pair.Key);
        }

        //specific message cleanup
        foreach (KeyValuePair<Pair<string, GameObject>, Delegate> pair in eventsSpecific)
        {
            bool wasFound = false;

            foreach (string message in permanentMessages)
            {
                if (pair.Key.First == message)
                {
                    wasFound = true;
                    break;
                }
            }

            if (!wasFound)
                messagesToRemove.Add(pair.Key.First);
        }

        foreach (string message in messagesToRemove)
        {
            events.Remove(message);
        }
    }

    #region adding a generic listener
    static void preAddListener(string eventType, Delegate listenerToAdd)
    {
        // if we havent added an event for this type yet, add a null k,v pair in the dictionary
        if (events.ContainsKey(eventType) == false)
        {
            events.Add(eventType, null);
        }

        // grab pre-existing event; null is returned if we just added the event
        Delegate del = events[eventType];

        // check if the event even contained anything
        if (listenerToAdd == null)
        {
            throw new ListenerException("You tried to add an empty event");
        }

        // make sure the listener to be added, and the current listeners to that event, are of the same type
        if (del != null && del.GetType() != listenerToAdd.GetType())
        {
            throw new ListenerException(
                "Attempting to add a listener with a type inconsistant with the current listeners for type "
                + eventType + ". Current type is " + del.GetType().Name + ", trying to add " + listenerToAdd.GetType().Name);
        }
    }



    static public void AddListener(string eventType, Callback listenerToAdd)
    {
        preAddListener(eventType, listenerToAdd);

        events[eventType] = (Callback)events[eventType] + listenerToAdd;
    }

    static public void AddListener<T>(string eventType, Callback<T> listenerToAdd)
    {
        preAddListener(eventType, listenerToAdd);

        events[eventType] = (Callback<T>)events[eventType] + listenerToAdd;
    }

    static public void AddListener<T, U>(string eventType, Callback<T, U> listenerToAdd)
    {
        preAddListener(eventType, listenerToAdd);

        events[eventType] = (Callback<T, U>)events[eventType] + listenerToAdd;
    }

    static public void AddListener<T, U, V>(string eventType, Callback<T, U, V> listenerToAdd)
    {
        preAddListener(eventType, listenerToAdd);

        events[eventType] = (Callback<T, U, V>)events[eventType] + listenerToAdd;
    }
    #endregion

    #region adding a specific listener
    static void preAddListenerSpecific(GameObject listenFor, Pair<string, GameObject> key, Delegate listenerToAdd)
    {
        // if we havent added an event for this type yet, add a null k,v pair in the dictionary

        if (eventsSpecific.ContainsKey(key) == false)
        {
            eventsSpecific.Add(key, null);
        }

        // grab pre-existing event; null is returned if we just added the event
        Delegate del = eventsSpecific[key];

        // check if the event even contained anything
        if (listenerToAdd == null)
        {
            throw new ListenerException("You tried to add an empty event");
        }

        // make sure the listener to be added, and the current listeners to that event, are of the same type
        if (del != null && del.GetType() != listenerToAdd.GetType())
        {
            throw new ListenerException(
                "Attempting to add a listener with a type inconsistant with the current listeners for type " 
                + key.ToString() + ". Current type is " 
                + del.GetType().Name + ", trying to add " 
                + listenerToAdd.GetType().Name);
        }
    }

    static public void AddListenerSpecific(GameObject listenFor, string eventType, Callback listenerToAdd)
    {
        Pair<string, GameObject> key = new Pair<string, GameObject>(eventType, listenFor);
        
        preAddListenerSpecific(listenFor, key, listenerToAdd);

        eventsSpecific[key] = (Callback)eventsSpecific[key] + listenerToAdd;
    }

    static public void AddListenerSpecific<T>(GameObject listenFor, string eventType, Callback<T> listenerToAdd)
    {
        Pair<string, GameObject> key = new Pair<string, GameObject>(eventType, listenFor);

        preAddListenerSpecific(listenFor, key, listenerToAdd);

        eventsSpecific[key] = (Callback<T>)eventsSpecific[key] + listenerToAdd;
    }

    static public void AddListenerSpecific<T,U>(GameObject listenFor, string eventType, Callback<T,U> listenerToAdd)
    {
        Pair<string, GameObject> key = new Pair<string, GameObject>(eventType, listenFor);

        preAddListenerSpecific(listenFor, key, listenerToAdd);

        eventsSpecific[key] = (Callback<T,U>)eventsSpecific[key] + listenerToAdd;
    }

    static public void AddListenerSpecific<T,U,V>(GameObject listenFor, string eventType, Callback<T,U,V> listenerToAdd)
    {
        Pair<string, GameObject> key = new Pair<string, GameObject>(eventType, listenFor);

        preAddListenerSpecific(listenFor, key, listenerToAdd);

        eventsSpecific[key] = (Callback<T,U,V>)eventsSpecific[key] + listenerToAdd;
    }

    #endregion

    #region removing a generic listener

    static void preRemoveListener(string eventType, Delegate listenerToRemove)
    {
        // check to see if this event even exists
        if (events.ContainsKey(eventType) == true)
        {
            // grab pre-existing event; null is returned if said event doesnt exist
            Delegate del = events[eventType];

            // check if any events of this type already exist (may be redundant, but whatever)
            if (del == null)
            {
                throw new ListenerException("Attempting to remove a listener for " + eventType + " but it doesn't exist");
            }

            // check for event type mismatch
            else if (del.GetType() != listenerToRemove.GetType())
            {
                throw new ListenerException(
                    "Attempting to remove a listener with a type inconsistant with the current listeners for type "
                    + eventType + ". Current type is " + del.GetType().Name + ", trying to add " + listenerToRemove.GetType().Name);
            }
        }


        else
        {
            throw new ListenerException("Attempting to remove listener for type: " + eventType + " but the messenger doesn't know about this event type.");
        }
    }

    static void postRemoveListener(string eventType)
    {
        if (events[eventType] == null)
        {
            events.Remove(eventType);
        }
    }


    static public void RemoveListener(string eventType, Callback listenerToRemove)
    {
        preRemoveListener(eventType, listenerToRemove);

        events[eventType] = (Callback)events[eventType] + listenerToRemove;

        postRemoveListener(eventType);
    }

    static public void RemoveListener<T>(string eventType, Callback<T> listenerToRemove)
    {
        preRemoveListener(eventType, listenerToRemove);

        events[eventType] = (Callback<T>)events[eventType] + listenerToRemove;

        postRemoveListener(eventType);
    }

    static public void RemoveListener<T, U>(string eventType, Callback<T, U> listenerToRemove)
    {
        preRemoveListener(eventType, listenerToRemove);

        events[eventType] = (Callback<T, U>)events[eventType] + listenerToRemove;

        postRemoveListener(eventType);
    }

    static public void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> listenerToRemove)
    {
        preRemoveListener(eventType, listenerToRemove);

        events[eventType] = (Callback<T, U, V>)events[eventType] + listenerToRemove;

        postRemoveListener(eventType);
    }

    #endregion

    #region broadcasting generic

    /// <summary>
    /// broadcasts a message with no parameters
    /// </summary>
    /// <param name="eventType"></param>
    static public void Broadcast(string eventType)
    {
        if (events.ContainsKey(eventType) == false)
        {
            throw new BroadcastException("No listener for event: " + eventType + " found.");

        }

        Delegate del;

        if (events.TryGetValue(eventType, out del) == true)
        {
            Callback callback = (Callback)del;

            if (callback != null)
            {
                callback();
            }

            else
            {
                throw new BroadcastException("Empty event found for " + eventType);
            }
        }
    }

    /// <summary>
    /// broadcasts a message with one parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="arg0"></param>
    static public void Broadcast<T>(string eventType, T arg0)
    {
        if (events.ContainsKey(eventType) == false)
        {
            throw new BroadcastException("No listener for event " + eventType + " found.");
        }

        Delegate del;

        if (events.TryGetValue(eventType, out del) == true)
        {
            Callback<T> callback = (Callback<T>)del;

            if (callback != null)
            {
                callback(arg0);
            }
            else
            {
                throw new BroadcastException("Empty event found for " + eventType);
            }
        }
    }

    /// <summary>
    /// broadcasts a message with two parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    static public void Broadcast<T, U>(string eventType, T arg0, U arg1)
    {
        if (events.ContainsKey(eventType) == false)
        {
            throw new BroadcastException("No listener for event " + eventType + "found.");
        }

        Delegate del;

        if (events.TryGetValue(eventType, out del) == true)
        {
            Callback<T, U> callback = (Callback<T, U>)del;

            if (callback != null)
            {
                callback(arg0, arg1);
            }
            else
            {
                throw new BroadcastException("Empty event found for " + eventType);
            }
        }
    }

    /// <summary>
    /// broadcasts a message with three parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="eventType"></param>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    static public void Broadcast<T,U,V>(string eventType, T arg0, U arg1, V arg2)
    {
        if (events.ContainsKey(eventType) == false)
        {
            throw new BroadcastException("No listener for event " + eventType + " found.");
        }

        Delegate del;

        if (events.TryGetValue(eventType, out del) == true)
        {
            Callback<T, U, V> callback = (Callback<T, U, V>)del;

            if (callback != null)
            {
                callback(arg0, arg1, arg2);
            }
            else
            {
                throw new BroadcastException("Empty event found for " + eventType);
            }
        }
    }

    #endregion

    #region broadcasting specific
    static public void BroadcastSpecific(GameObject broadcaster, string eventType)
    {
        Pair<string, GameObject> key = new Pair<string, GameObject>(eventType, broadcaster);
        foreach (Pair<string, GameObject> item in eventsSpecific.Keys)
        {
            Debug.Log(item.ToString());
        }
        if (eventsSpecific.ContainsKey(key) == false)
        {
            throw new BroadcastException("No listener for event: " + key.ToString() + " found.");
        }

        Delegate del;

        if (eventsSpecific.TryGetValue(key, out del) == true)
        {
            Callback callback = (Callback)del;

            if (callback != null)
            {
                callback();
            }
            else
            {
                throw new BroadcastException("Empty event found for " + key.ToString());
            }
        }
    }

    static public void BroadcastSpecific<T>(GameObject broadcaster, string eventType, T arg0)
    {
        Pair<string, GameObject> key = new Pair<string, GameObject>(eventType, broadcaster);
        foreach (Pair<string, GameObject> item in eventsSpecific.Keys)
        {
            Debug.Log(item.ToString());
        }
        if (eventsSpecific.ContainsKey(key) == false)
        {
            throw new BroadcastException("No listener for event: " + key.ToString() + " found.");
        }

        Delegate del;

        if (eventsSpecific.TryGetValue(key, out del) == true)
        {
            Callback<T> callback = (Callback<T>)del;

            if (callback != null)
            {
                callback(arg0);
            }
            else
            {
                throw new BroadcastException("Empty event found for " + key.ToString());
            }
        }
    }

    static public void BroadcastSpecific<T,U>(GameObject broadcaster, string eventType, T arg0, U arg1)
    {
        Pair<string, GameObject> key = new Pair<string, GameObject>(eventType, broadcaster);
        foreach (Pair<string, GameObject> item in eventsSpecific.Keys)
        {
            Debug.Log(item.ToString());
        }
        if (eventsSpecific.ContainsKey(key) == false)
        {
            throw new BroadcastException("No listener for event: " + key.ToString() + " found.");
        }

        Delegate del;

        if (eventsSpecific.TryGetValue(key, out del) == true)
        {
            Callback<T,U> callback = (Callback<T,U>)del;

            if (callback != null)
            {
                callback(arg0, arg1);
            }
            else
            {
                throw new BroadcastException("Empty event found for " + key.ToString());
            }
        }
    }

    static public void BroadcastSpecific<T,U,V>(GameObject broadcaster, string eventType, T arg0, U arg1, V arg2)
    {
        Pair<string, GameObject> key = new Pair<string, GameObject>(eventType, broadcaster);
        foreach (Pair<string, GameObject> item in eventsSpecific.Keys)
        {
            Debug.Log(item.ToString());
        }
        if (eventsSpecific.ContainsKey(key) == false)
        {
            throw new BroadcastException("No listener for event: " + key.ToString() + " found.");
        }

        Delegate del;

        if (eventsSpecific.TryGetValue(key, out del) == true)
        {
            Callback<T,U,V> callback = (Callback<T,U,V>)del;

            if (callback != null)
            {
                callback(arg0, arg1, arg2);
            }
            else
            {
                throw new BroadcastException("Empty event found for " + key.ToString());
            }
        }
    }
    #endregion
}

//This manager will ensure that the messenger's eventTable will be cleaned up upon loading of a new level.
public sealed class MessengerHelper : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    //Clean up eventTable every time a new level loads.
    public void OnLevelWasLoaded(int unused)
    {
        Debug.Log("cleaning up messages...");
        Messenger.Cleanup();
    }
}



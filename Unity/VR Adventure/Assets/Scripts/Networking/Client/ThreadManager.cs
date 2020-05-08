using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadManager : MonoBehaviour
{
    private static readonly List<Action> executeOnMainThread = new List<Action>();
    private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
    private static bool actionToExecuteOnMainThread = false;

    // Update is called once per frame
    void Update()
    {
        UpdateMain();
    }

    /// <summary>
    /// Sets and actions to be executed on the main thread!
    /// </summary>
    /// <param name="_action"></param>
    public static void ExecuteOnMainThread(Action _action)
    {
        if(_action != null)
        {
            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(_action); //Add the action to the list of actions to be executed
                actionToExecuteOnMainThread = true; //Set update loop that there's new actions to execute
            }
        }
    }

    /// <summary>
    /// Execute actions set to run on the main thread, should ONLY be called from the main thread
    /// </summary>
    public static void UpdateMain()
    {
        if(actionToExecuteOnMainThread)
        {
            executeCopiedOnMainThread.Clear();
            lock(executeOnMainThread)
            {
                executeCopiedOnMainThread.AddRange(executeOnMainThread);
                executeOnMainThread.Clear();
                actionToExecuteOnMainThread = false;
            }
        }

        for (int i = 0; i < executeCopiedOnMainThread.Count; i++)
        {
            executeCopiedOnMainThread[i]();
        }
    }

}

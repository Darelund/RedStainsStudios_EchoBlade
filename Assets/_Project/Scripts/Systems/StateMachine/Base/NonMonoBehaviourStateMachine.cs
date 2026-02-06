using System;
using System.Collections.Generic;
using UnityEngine;

public class NonMonoBehaviourStateMachine : MonoBehaviour
{
    [NonSerialized] protected List<NonMonoState> states = new List<NonMonoState>();
    [NonSerialized] private NonMonoState Currentstate;
    public string NameOfCurrentState;
    public void SwitchState<aState>()
    {
        foreach (NonMonoState s in states)
        {
            if (s.GetType() == typeof(aState))
            {
                Currentstate?.ExitState();
                Currentstate = s;
                NameOfCurrentState = s.GetType().Name;
                Currentstate.EnterState();
                return;
            }
        }
        Debug.LogWarning("state does not exist");
    }

    public virtual void UpdateStateMachine()
    {
        Currentstate?.UpdateState();
    }
    public virtual void FixedUpdateStateMachine()
    {
        Currentstate?.FixedUpdateState();
    }

    public NonMonoState GetCurrentState()
    {
        return Currentstate;
    }

    public bool IsState<aState>()
    {
        if (Currentstate == null) return false;
        return Currentstate.GetType() == typeof(aState);
    }


}

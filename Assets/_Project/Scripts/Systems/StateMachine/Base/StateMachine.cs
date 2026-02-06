using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private List<State> states = new List<State>();
    [HideInInspector] public State Currentstate = null;


    public void SwitchState<aState>()
    {
        foreach (State s in states)
        {
            if (s.GetType() == typeof(aState))
            {
                Currentstate?.ExitState();
                Currentstate = s;
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

    public State GetCurrentState()
    {
        return Currentstate;
    }

    public bool IsState<aState>()
    {
        if (!Currentstate) return false;
        return Currentstate.GetType() == typeof(aState);
    }


}

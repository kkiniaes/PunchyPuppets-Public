using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM {
    private State[] states;
    private State curState;

    public FSM(State[] _states)
    {
        states = _states;
        if (states.Length > 0)
        {
            curState = states[0];
            curState.Enter();
        }
    }

    public void Run()
    {
        curState.Run();
    }

    public void ChangeState(State toState)
    {
        if(curState != null) curState.Exit();
        curState = toState;
        if (curState != null) curState.Enter();
    }
}

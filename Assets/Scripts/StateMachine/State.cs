using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State {
    public delegate void StateAction();

    private StateAction run, enter, exit;

    public State(StateAction runAction, StateAction enterAction, StateAction exitAction)
    {
        run = runAction;
        enter = enterAction;
        exit = exitAction;
    }

    public void Run()
    {
        if (run != null) run();
    }

    public void Enter()
    {
        if (enter != null) enter();
    }

    public void Exit()
    {
        if (exit != null) exit();
    }
}

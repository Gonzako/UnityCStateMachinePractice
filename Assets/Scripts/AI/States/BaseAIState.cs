using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseAIState
{
    public abstract AIManager _ai { get; set; }
    public abstract Type Tick();
    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }

    public BaseAIState(AIManager ai)
    {
        _ai = ai;
    }
}


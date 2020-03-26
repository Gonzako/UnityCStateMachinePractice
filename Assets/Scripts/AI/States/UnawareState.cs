using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnawareState : BaseAIState
{
    public override AIManager _ai { get; set; }


    public UnawareState(AIManager ai) : base(ai)
    {
        _ai = ai;
    }

    public override void OnStateEnter()
    {
        _ai.Patrol();
    }

    public override Type Tick()
    {
        if(_ai.GetComponent<AIScanner>()._visibleTargets.Count > 0)
        {
            return typeof(AgressiveState);
        }else if(_ai._listener.heardPlayer())
        {
            return typeof(AwareState);
        }
        else
        {
            return null;
        }

    }

  
}

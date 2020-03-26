using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwareState : BaseAIState
{
    public override AIManager _ai { get; set; }

    public AwareState(AIManager ai) : base(ai)
    {
        _ai = ai;
    }

    public override void OnStateEnter()
    {
        _ai.StopAllCoroutines();
        _ai.StartCoroutine(_ai.Search());
        _ai.StartCoroutine(_ai.Wait());

    }
    public override Type Tick()
    {
        if (!_ai.canSearch)
        {
            return typeof(UnawareState);
        }
        else
        {
            return null;
        }
    }
}

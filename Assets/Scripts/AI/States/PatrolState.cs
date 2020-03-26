using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : BaseAIState {

    public override AIManager _ai { get; set; }


    public PatrolState(AIManager ai) : base(ai)
    {
        _ai = ai;
    }

    public override void OnStateEnter()
    {
       
    }

    public override Type Tick()
    {
        if (Vector2.Distance(_ai.transform.position, _ai._Player.transform.position) < 1F)
        {
            return null;
        }
        else
        {

            return null;
        }
    }
}

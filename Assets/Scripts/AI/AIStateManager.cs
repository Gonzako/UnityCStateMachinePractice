using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AIStateManager : MonoBehaviour
{
    private BaseAIState _currentState;

    private BaseAIState _nextState;

    Dictionary<Type, BaseAIState> _availableStates;

    public event Action<BaseAIState> onStateChanged;

    void Update()
    {
        if (_currentState == null && _availableStates != null)
        {
            SwitchToNewState(_availableStates.Keys.First());
        }

        //Take in next state
        var nextState = _currentState?.Tick();

        if (nextState != null && nextState != _currentState?.GetType())
        {
            SwitchToNewState(nextState);
        }
    }


    /// <summary>
    /// Sets currently available states
    /// </summary>
    /// <param name="states"></param>
    public void SetStates(Dictionary<Type, BaseAIState> states)
    {
        Debug.Log("Set State");
        this._availableStates = states;
    }

    /// <summary>
    /// Switches to new state in the state dictionary
    /// </summary>
    /// <param name="nextstate"></param>
    void SwitchToNewState(Type nextstate)
    {
        _currentState?.OnStateExit();
        _currentState = _availableStates[nextstate];
        onStateChanged?.Invoke(_currentState);
        _currentState?.OnStateEnter();
        Debug.LogFormat("{0} state Changed To: " + _currentState.ToString(), gameObject.name);
    }
}

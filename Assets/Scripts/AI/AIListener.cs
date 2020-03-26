using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIListener : MonoBehaviour
{
    private LinkedList<Vector2> _audiblePosition = new LinkedList<Vector2>();
    private float _refreshTime = 5.0F;

    private void Start()
    {
        foreach (AiAttentionCaller caller in AiCallerTracker.instance.attentionCallers)
        {
            caller.onMakeNoise += newPosition;
        }

        StartCoroutine(refreshPositions());
    }

    private void newPosition(GameObject guy)
    {
        _audiblePosition.AddFirst(guy.transform.position);
    }

    public Vector2 getMostRecentPosition()
    {
        Vector2 rs = _audiblePosition.First.Value;
        return rs;
    }


    private IEnumerator refreshPositions()
    {
        while (true)
        {
            yield return new WaitForSeconds(_refreshTime);
            if(_audiblePosition.Last != null)
            _audiblePosition.RemoveLast();
        }
    }

    public bool heardPlayer()
    {
        return (_audiblePosition.Count > 0);
    }
}

using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementModule : MonoBehaviour
{
    public Vector3 _target;
    private ABPath _path;
    private Seeker _seeker;
    private bool isTraversing;
    IAstarAI _agent;

    private void Start()
    {
        _agent = GetComponent<IAstarAI>();
        _seeker = GetComponent<Seeker>();
    }
    public void TraversalToTarget()
    {

        _path = ABPath.Construct(transform.position, _target);
        _agent.SetPath(_path);
        _seeker.StartPath(transform.position, _target);
      
       

       
    }

}

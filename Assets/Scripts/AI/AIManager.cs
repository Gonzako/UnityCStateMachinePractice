using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.AI;

public class AIManager : MonoBehaviour
{
    private Dictionary<Type, BaseAIState> _initialStates;
    public GameObject _Player;
    private AIStateManager _stateManager;

    [SerializeField] public Vector3 _target;
    public Rigidbody2D _rb;

    public Path path;
    public Vector3 velocity;

    public float nextWaypointDistance = 3;
    private int currentWaypoint = 0;
    public bool reachedEndOfPath;
    Seeker seeker;
    public Vector2 _lastKnownPos;

    public AISettings _settings;

    public AIListener _listener;

    public bool canSearch = true;

    private void Start()
    {
        SetupStates();

        _rb = GetComponent<Rigidbody2D>();
        _listener = GetComponent<AIListener>();

    }

    private void SetupStates()
    {
        
        _stateManager = GetComponent<AIStateManager>();
        _initialStates = new Dictionary<Type, BaseAIState>
        {
            {typeof(UnawareState), new UnawareState(this)},
            {typeof(AwareState), new AwareState(this)},
            {typeof(PatrolState), new PatrolState(this)},
            {typeof(AgressiveState), new AgressiveState(this)}
        };
        _stateManager.SetStates(_initialStates);
        
    }

    void Update()
    {
        if (path == null)
        {
            return;
        }
        float distanceToWaypoint;
        reachedEndOfPath = false;
        while (true)
        {
          
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    reachedEndOfPath = true;
                    break;
                }
            }
            else
            {
                break;
            }

   
            var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;

            velocity = dir * _settings._movementSpeed * speedFactor;
            transform.position += velocity * Time.deltaTime;
        }
     
    }

    public void ChasePlayer()
    {
        this._target = _Player.transform.position;
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("A path was calculated. Did it fail with an error? " + p.error);

        if (!p.error)
        {
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }

    }

    public void SetDestination(Vector2 dest)
    {
            seeker.StartPath(transform.position, dest, OnPathComplete);    
    }

    public Vector2 RandomPosition()
    {
        return (Vector2)new Vector2(UnityEngine.Random.Range(0.0F, 5.0F),
             UnityEngine.Random.Range(0.0F, 5.0F));
    }

    public void Patrol()
    {
        if (_settings._patrolPoints.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(PatrolWaypoints());
        }
    }

    public void ChaseTarget(Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(ChasePlayer(target));
    }
   
    IEnumerator PatrolWaypoints()
    {
        int currentWaypoint = 0;
        while (true)
        {
            if(currentWaypoint == _settings._patrolPoints.Count)
            {
                currentWaypoint = 0;
            }

            SetDestination(_settings._patrolPoints[currentWaypoint].position);
            yield return new WaitUntil(() => reachedEndOfPath);
            yield return new WaitForSeconds(2F);

            currentWaypoint++;
        }
    }

    IEnumerator ChasePlayer(Transform target)
    {
        while (true)
        {
            SetDestination(target.position);
            //Wait for end of path and then wait for 0.2 milliseconds:
            yield return new WaitUntil(() => reachedEndOfPath);
            yield return new WaitForSeconds(0.2F);
        }
    }

    public IEnumerator Search()
    {
        while (canSearch)
        {
            if (_listener.heardPlayer())
            {
                SetDestination(_listener.getMostRecentPosition());
                yield return new WaitUntil(() => reachedEndOfPath);
                yield return new WaitForSeconds(2F);
            }
            else
            {
                SetDestination(RandomPosition());
                yield return new WaitUntil(() => reachedEndOfPath);
                yield return new WaitForSeconds(5F);
                
            }
        }
    }


   public IEnumerator Wait()
    {
        canSearch = true;
        yield return new WaitForSeconds(_settings.searchWaitTime);
        canSearch = false;
    }
}
    


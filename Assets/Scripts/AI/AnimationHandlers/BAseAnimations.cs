using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimations : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    private AIManager _lerp;

    private void Start()
    {
        _lerp = GetComponent<AIManager>();
    }
    private void Update()
    {
        _anim.SetFloat("velocity", _lerp.velocity.magnitude);
    }


}

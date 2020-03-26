using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIListenerSingleton : MonoBehaviour
{

    AIListener[] _listeners;

    // Start is called before the first frame update
    void Start()
    {
        _listeners = FindObjectsOfType<AIListener>();
    }

}

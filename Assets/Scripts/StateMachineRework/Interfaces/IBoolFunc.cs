using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBoolFunc
{
    bool evaluate(float val1, float val2, params string[] input);
}

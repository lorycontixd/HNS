using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDbListener 
{
    public void OnQuery(ResultType type, QueryData data);
}

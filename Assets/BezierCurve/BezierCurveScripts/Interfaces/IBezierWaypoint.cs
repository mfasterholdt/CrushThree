/***************************************************************************************
 * Copyright 2010 Friction Point Studios Pty Ltd                                      
 ***************************************************************************************/

using UnityEngine;
using System.Collections;

public interface IBezierWaypoint
{
    IBezierControlPoint LeftPoint
    {
        get;
    }

    IBezierControlPoint RightPoint
    {
        get;
    }

    Vector3 CurrentPosition
    {
        get;
    }
}


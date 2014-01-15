/***************************************************************************************
 * Copyright 2010 Friction Point Studios Pty Ltd                                      
 ***************************************************************************************/

using UnityEngine;
using System.Collections;

public interface IBezierControlPoint
{
    BezierControlPointSide Side { get; }

    Vector3 CurrentPosition { get; set; }
}


/***************************************************************************************
 * Copyright 2010 Friction Point Studios Pty Ltd                                      
 ***************************************************************************************/

using UnityEngine;
using System.Collections;

public interface IBezierCurveManager
{
    /// <summary>
    /// Gets a position along the curve 
    /// </summary>
    /// <param name="time">Time, with 0 being the start of the curve and SecondsForFullLoop being the end</param>
    /// <returns>The point along the curve representing time</returns>
    Vector3 GetPositionAtTime(float time);

    /// <summary>
    /// Gets a position along the curve offset by a distance. Note: This is an approximation and should be used 
    /// in conjunction with Lerp to generate a smooth result. 
    /// </summary>
    /// <param name="distance">Distance in units along the curve that the position should be offset by</param>
    /// <param name="time">Time, with 0 being the start of the curve and SecondsForFullLoop being the end</param>
    /// <returns></returns>
    Vector3 GetPositionAtDistance(float distance, float time);
}


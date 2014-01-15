/***************************************************************************************
 * Copyright 2010 Friction Point Studios Pty Ltd                                      
 ***************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MyBezier
{
    List<IBezierWaypoint> waypointList;
    
    Vector3[] vectorArray;

    /// <summary>
    /// This is the number of points that we calculate for the distance offset stuff. The
    /// higher the number means more precision, but a longer time at start up to calucate 
    /// them all. 
    /// </summary>
    const int NumberOfArray = 1000;
    const float OneOverNum = 1f / (float)NumberOfArray;

    float[] distanceArray = new float[NumberOfArray];
    float[] timeArray = new float[NumberOfArray];

    /// <summary>
    /// This is the length of the curve based on the number of distance calculations. 
    /// </summary>
    float MaxDistance = 0.0f;

    /// <summary>
    /// Constructor for MyBezier
    /// </summary>
    /// <param name="waypointsArray">The array of IBezierWaypoints that make up the bezier curve</param>
    public MyBezier(IBezierWaypoint[] waypointsArray)
    {
        waypointList = new List<IBezierWaypoint>();

        // Unity iPhone doesn't seem to like me using Add range. 
        foreach (IBezierWaypoint waypoint in waypointsArray)
        {
            waypointList.Add(waypoint);
        }
    }

    /// <summary>
    /// Sets up the Distance and Time arrays for use in following the curve at a fixed distance from another object. 
    /// </summary>
    /// <param name="fullLoop">Is the curve a full loop</param>
    public void SetUpDistanceLists(bool fullLoop)
    {
        distanceArray[0] = 0.0f;
        timeArray[0] = 0.0f;

        Vector3 previousPoint = this.GetPointAtTime(0, fullLoop);

        Vector3 newPoint = Vector3.zero;

        float totalDistance = 0.0f;

        for (int t = 1; t < NumberOfArray; t += 1)
        {
            newPoint = this.GetPointAtTime(t * OneOverNum, fullLoop);

            totalDistance += (previousPoint - newPoint).magnitude;

            distanceArray[t] = totalDistance;
            timeArray[t] = t * OneOverNum;
            previousPoint = newPoint;
        }

        this.MaxDistance = totalDistance;
    }

    /// <summary>
    /// Get a point on the Bezier curve at a time
    /// </summary>
    /// <param name="t">Time between 0 and 1. 0 is the first waypoint, 1 is the last waypoint (or the fist one if a full loop)</param>
    /// <param name="fullLoop">Is the curve a full loop</param>
    /// <returns>The point on the curve at the specified time</returns>
    public Vector3 GetPointAtTime(float t, bool fullLoop)
    {
        t = t % 1.0f;

        if (t < 0)
        {
            t = 1.0f + t;
        }

        // This is the segment that we are currently in along the curve. 
        int numToUse;

        if (fullLoop)
        {
            numToUse = waypointList.Count;
        }
        else
        {
            numToUse = waypointList.Count - 1;
        }
        int x = Mathf.FloorToInt(t * (float)numToUse);

        float tBetweenZeroAndOne = (t * (float)numToUse) - (float)x;

        Vector3 currentPoint = waypointList[CheckWithinArray(x, waypointList.Count)].CurrentPosition;
        Vector3 currentRightControlPoint = waypointList[CheckWithinArray(x, waypointList.Count)].RightPoint.CurrentPosition;
        Vector3 nextLeftControlPoint = waypointList[CheckWithinArray(x + 1, waypointList.Count)].LeftPoint.CurrentPosition;
        Vector3 nextPoint = waypointList[CheckWithinArray(x + 1, waypointList.Count)].CurrentPosition;

        return DoBezierFor4Points(tBetweenZeroAndOne, currentPoint, currentRightControlPoint, nextLeftControlPoint, nextPoint);
    }

    private int CheckWithinArray(int x, int c)
    {
        if (x >= c)
        {
            return x % c;
        }
        else
        {
            return x;
        }
    }

    private Vector3 DoBezierFor4Points(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 returnVector = Vector3.zero;

        float oneMinusT = (1f - t);

        returnVector += p0 * oneMinusT * oneMinusT * oneMinusT;

        returnVector += p1 * t * 3f * oneMinusT * oneMinusT;

        returnVector += p2 * 3f * t * t * oneMinusT;

        returnVector += p3 * t * t * t;

        return returnVector;
    }

    /// <summary>
    /// Find the time point (0...1) that the distance represents   
    /// </summary>
    /// <param name="distance">Distance along the curve</param>
    /// <returns>The time that the distnace represents</returns>
    public float FindTimePointAlongeSplineAtDistance(float distance)
    {
        distance = distance % MaxDistance;

        if (distance < 0)
        {
            distance = MaxDistance + distance;
        }


        float timeValue = 0.0f; ;

        for (int i = 0; i < NumberOfArray; i++)
        {
            timeValue = timeArray[i];

            if (distance < distanceArray[i])
            {
                break;
            }

        }

        return timeValue;
    }

    /// <summary>
    /// Look up the distance value for a given time point
    /// </summary>
    /// <param name="time">The time point around the curve (0..1)</param>
    /// <returns>The distance around the curve</returns>
    public float LookupDistanceOfExistingTime(float time)
    {
        time = time % 1.0f;

        float posX = 0.0f;

        for (int i = 0; i < NumberOfArray; i++)
        {
            posX = distanceArray[i];

            if (time < timeArray[i])
            {
                break;
            }

        }

        return posX;
    }
}

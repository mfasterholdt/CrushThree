/***************************************************************************************
 * Copyright 2010 Friction Point Studios Pty Ltd                                      
 ***************************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BezierCurveManager : MonoBehaviour, IBezierCurveManager
{
    private List<IBezierWaypoint> waypointList = new List<IBezierWaypoint>();

    private MyBezier bezier;

    /// <summary>
    /// If checked, this BezierCurveManager can be used to call GetPositionAtDistance. 
    /// Additional setup is required so this must be set at startup. 
    /// </summary>
    public bool EnableDistanceCalculations;

    /// <summary>
    /// If checked, the Gizmos showing the curve will be shown in the editor
    /// </summary>
    public bool DrawGizmos = true;

    /// <summary>
    /// If checked, the Gizmos showing the control points will be shown in the editor
    /// </summary>
    public bool DrawControlPoints = true;

    /// <summary>
    /// If checked, the curve runs from the first waypoint to the first waypoint, rather than ending at the last waypoint
    /// </summary>
    public bool IsFullLoop;

    /// <summary>
    /// The number of seconds for a full loop of the curve
    /// </summary>
    public float SecondsForFullLoop = 1.0f;

    void Awake()
    {
        foreach (IBezierWaypoint comp in gameObject.GetComponentsInChildren(typeof(IBezierWaypoint)))
        {
            this.waypointList.Add(comp);
        }
        this.bezier = new MyBezier(this.waypointList.ToArray());
    }

    void Start()
    {
        if (this.EnableDistanceCalculations)
        {
            this.bezier.SetUpDistanceLists(this.IsFullLoop);
        }
    }

    public Vector3 GetPositionAtTime(float time)
    {
        time = time / this.SecondsForFullLoop;

        return this.bezier.GetPointAtTime(time, this.IsFullLoop);
    }

    public Vector3 GetPositionAtDistance(float distance, float time)
    {
        if (this.EnableDistanceCalculations)
        {
            time = time / this.SecondsForFullLoop;

            float posx = this.bezier.LookupDistanceOfExistingTime(time);
            float timePoint = this.bezier.FindTimePointAlongeSplineAtDistance(distance + posx);
            return this.bezier.GetPointAtTime(timePoint, this.IsFullLoop);
        }
        else
        {
            throw new Exception("In order to use GetPositionAtDistance the EnableDistanceCalculation " +
                                "variable must be set to true at start up so the distance points can " + 
                                "be pre-calculated");
        }
    }

    public void OnDrawGizmos()
    {
        // This actually calculates 1000 points along the bezier curve each time the gizmos update. 
        // It seems like this would kill unity but the calculations are suprisingly fast and it doesn't 
        // seem to cause a problem. It might be an issue if you have lots of these but you can turn them 
        // off using DrawGizmos. 
        if (this.DrawGizmos)
        {
            List<BezierWaypoint> gizmoWaypointList = new List<BezierWaypoint>();

            foreach (BezierWaypoint comp in gameObject.GetComponentsInChildren(typeof(BezierWaypoint)))
            {
                comp.SetControlPoints();
                if (comp.IsValid)
                {
                    gizmoWaypointList.Add(comp);
                }
            }

            if (gizmoWaypointList.Count != 0)
            {
                MyBezier gizmoBezier = new MyBezier(gizmoWaypointList.ToArray());

                for (float t = 0.0f; t < 1.0f; t += 0.001f)
                {
                    Gizmos.DrawIcon(gizmoBezier.GetPointAtTime(t, this.IsFullLoop), "/Bezier/BezierCurve.png");
                }
            }
        }
    }
}


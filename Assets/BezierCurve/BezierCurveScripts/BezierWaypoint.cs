/***************************************************************************************
 * Copyright 2010 Friction Point Studios Pty Ltd                                      
 ***************************************************************************************/

using UnityEngine;
using System.Collections;

/// <summary>
/// This is a waypoint which makes up the bezier curve. Under this in the Heirachy is a left and 
/// a right control point. In a nutshell, they control how far we move away from this waypoint before
/// turning toward the next one, but look at wikipedia for more detailed info. 
/// </summary>
public class BezierWaypoint : MonoBehaviour, IBezierWaypoint
{
    void Awake()
    {
        this.SetControlPoints();
    }

    public void SetControlPoints()
    {
        foreach (IBezierControlPoint controlPoint in this.GetComponentsInChildren(typeof(IBezierControlPoint)))
        {

            if (controlPoint.Side == BezierControlPointSide.Left)
            {
                this.LeftPoint = controlPoint;
            }
            else if (controlPoint.Side == BezierControlPointSide.Right)
            {
                this.RightPoint = controlPoint;
            }
            else
            {
                Debug.LogError("Bezier Curve control points must be set either left or right in the Editor");
            }

        }
    }

    /// <summary>
    /// If the waypoint is not valid then it will not be used in the curve calculations
    /// </summary>
    public bool IsValid
    {
        get
        {
            return (this.LeftPoint != null && this.RightPoint != null);
        }
    }

    /// <summary>
    /// When the control points are dragged around in the Editor, their OnDrawGizmos method calls
    /// this method, which aligns the opposite waypoint in the correct spot. 
    /// </summary>
    /// <param name="controlPoint"></param>
    /// <param name="vectorToFootPoint"></param>
    public void SetPositionOfOther(IBezierControlPoint controlPoint, Vector3 vectorToFootPoint)
    {
        if (this.RightPoint != null && this.LeftPoint != null)
        {
            vectorToFootPoint.Normalize();

            if (controlPoint.Side == BezierControlPointSide.Left)
            {
                float magOfVector = (this.CurrentPosition - this.RightPoint.CurrentPosition).magnitude;
                this.RightPoint.CurrentPosition = this.CurrentPosition + vectorToFootPoint * magOfVector;
            }
            else
            {
                float magOfVector = (this.CurrentPosition - this.LeftPoint.CurrentPosition).magnitude;
                this.LeftPoint.CurrentPosition = this.CurrentPosition + vectorToFootPoint * magOfVector;
            }
        }
    }

    private IBezierControlPoint leftPoint;
    public IBezierControlPoint LeftPoint
    {
        get
        {
            return this.leftPoint;
        }
        set
        {
            this.leftPoint = value;
        }
    }


    private IBezierControlPoint rightPoint;
    public IBezierControlPoint RightPoint
    {
        get
        {
            return this.rightPoint;
        }
        set
        {
            this.rightPoint = value;
        }
    }

    public Vector3 CurrentPosition
    {
        get
        {
            return transform.position;
        }
    }

    void OnDrawGizmos()
    {
        BezierCurveManager manager = this.transform.parent.GetComponent(typeof(BezierCurveManager)) as BezierCurveManager;
        if (this.IsValid &&  manager.DrawGizmos)
        {

            Gizmos.DrawIcon(transform.position, "/Bezier/BezierWaypoint.png");

            if (manager.DrawControlPoints)
            {
                SetControlPoints();

                if (this.RightPoint != null && this.LeftPoint != null)
                {
                    Gizmos.DrawLine(this.RightPoint.CurrentPosition, this.LeftPoint.CurrentPosition);
                }
            }
        }
    }

}


/***************************************************************************************
 * Copyright 2010 Friction Point Studios Pty Ltd                                      
 ***************************************************************************************/

using UnityEngine;
using System.Collections;

public class BezierControlPoint : MonoBehaviour, IBezierControlPoint
{

    public BezierControlPointSide side;
    public BezierControlPointSide Side
    {
        get
        {
            return this.side;
        }
    }

    public Vector3 CurrentPosition
    {
        get
        {
            return transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }

    void OnDrawGizmos()
    {
        if (this.transform.parent.parent != null)
        {
            BezierCurveManager manager = this.transform.parent.parent.GetComponent(typeof(BezierCurveManager)) as BezierCurveManager;
            if (manager.DrawControlPoints)
            {
                Gizmos.DrawIcon(transform.position, "/Bezier/BezierControlPoint.png");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (transform.parent != null)
        {
            Component controller = transform.parent.GetComponent(typeof(BezierWaypoint));
            if (controller != null && controller is BezierWaypoint)
            {
                BezierWaypoint footPoint = controller as BezierWaypoint;

                Vector3 vectorToFootPoint = footPoint.CurrentPosition - this.transform.position;

                footPoint.SetPositionOfOther(this, vectorToFootPoint);
            }
        }
    }
}

public enum BezierControlPointSide
{
    None,
    Left,
    Right
}


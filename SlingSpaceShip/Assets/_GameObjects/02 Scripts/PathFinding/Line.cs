using UnityEngine;

public struct Line
{
    private const float verticalLineGradient = 1e5f;
    
    public float gradient;
    public float yIntercept;
    
    public float gradientPerpendicular;

    private bool approachSide;
    
    private Vector2 pointOnLine1;
    private Vector2 pointOnLine2;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if (dx == 0)
        {
            gradientPerpendicular = verticalLineGradient;
        }
        else
        {
            gradientPerpendicular = dy / dx;   
        }

        if (gradientPerpendicular == 0)
        {
            gradient = verticalLineGradient;
        }
        else
        {
            gradient = -1 / gradientPerpendicular;
        }
        
        yIntercept = pointOnLine.y - gradient * pointOnLine.x;
        
        pointOnLine1 = pointOnLine;
        pointOnLine2 = pointOnLine + new Vector2(1, gradient);
        
        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }

    public bool GetSide(Vector2 point)
    {
        return (point.x - pointOnLine1.x) * (pointOnLine2.y - pointOnLine1.y) >  (point.y - pointOnLine1.y) * (pointOnLine2.x - pointOnLine1.x);
    }

    public bool HasCrossedLine(Vector2 point)
    {
        return GetSide(point) != approachSide;
    }

    public float DistFromPoint(Vector2 point)
    {
        float yInterceptPerpendicular = point.y - gradientPerpendicular * point.x;
        float intersectX = (yInterceptPerpendicular - yIntercept) / (gradient - gradientPerpendicular);
        float intersectY = gradient * intersectX + yIntercept;
        
        float dist = Vector2.Distance(point, new Vector2(intersectX, intersectY));
        return dist;
    }
    
    public void DrawWithGizmos(float length)
    {
        Gizmos.color = Color.blue;
        Vector3 lineDir = new Vector3(1.0f, gradient, 0.0f);
        lineDir.Normalize();
        
        Vector3 lineCenter = new Vector3(pointOnLine1.x, pointOnLine1.y, 0.0f);
        
        Gizmos.DrawLine(lineCenter - lineDir * length * 0.5f, lineCenter + lineDir * length * 0.5f);
    }
}

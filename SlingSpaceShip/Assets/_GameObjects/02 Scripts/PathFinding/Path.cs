using UnityEngine;

public class TravelPath
{
    public readonly Vector3[] lookPoints;
    public readonly Line[] turnBoundaries;
    public readonly int finishLineIndex;
    public readonly int slowDownIndex;

    public TravelPath(Vector3[] wayPoints, Vector3 startPoint, float turnDist, float stoppingDist)
    {
        lookPoints = wayPoints;
        turnBoundaries = new Line[lookPoints.Length];   
        finishLineIndex = turnBoundaries.Length - 1;
        
        Vector2 prevPoint = new Vector2(startPoint.x, startPoint.y);

        for (int i = 0; i < lookPoints.Length; i++)
        {
            Vector2 curPoint = new Vector2(lookPoints[i].x, lookPoints[i].y);
            Vector2 dir = curPoint - prevPoint;
            dir.Normalize();
            
            Vector2 turnBoundaryPoint = i == finishLineIndex ? curPoint : curPoint - dir * turnDist;
            
            turnBoundaries[i] = new Line(turnBoundaryPoint, prevPoint - dir * turnDist);
            
            prevPoint = turnBoundaryPoint;
        }

        float distFromEndPoint = 0.0f;

        for (int i = lookPoints.Length - 1; i > 0; i--)
        {
            distFromEndPoint += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);

            if (distFromEndPoint > stoppingDist)
            {
                slowDownIndex = i;
                break;
            }
        }
    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.cyan;

        for (int i = 0; i < lookPoints.Length; i++)
        {
            Gizmos.DrawCube(lookPoints[i], Vector3.one);
        }

        for (int i = 0; i < turnBoundaries.Length; i++)
        {
            turnBoundaries[i].DrawWithGizmos(10);
        }
    }
}

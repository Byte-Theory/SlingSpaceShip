using System;
using System.Collections.Generic;
using UnityEngine;


struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
}

public class PathRequestManager : MonoBehaviour
{
    [SerializeField] private PathFinding pathFinding;
    private bool isProcessingPath;
    
    private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    private PathRequest curRequest;

    #region SingleTon

    public static PathRequestManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion
    
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        Instance.pathRequestQueue.Enqueue(newRequest);
        Instance.TryProcessNext();
    }

    private void TryProcessNext()
    {
        if (isProcessingPath)
        {
            return;
        }

        if (pathRequestQueue.Count == 0)
        {
            return;
        }
        
        curRequest = pathRequestQueue.Dequeue();
        pathFinding.StartFindPath(curRequest.pathStart, curRequest.pathEnd);
        isProcessingPath = true;
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        curRequest.callback(path, success);
        isProcessingPath = false;
        
        TryProcessNext();
    }
}

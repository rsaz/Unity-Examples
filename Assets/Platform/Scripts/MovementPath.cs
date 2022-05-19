using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPath : MonoBehaviour
{
    public static event Action OnPointAdded;

    public enum PathTypes
    {
        Linear,
        Loop
    }

    public enum MoveDirection
    {
        Clockwise = 1,
        AntiClockwise = -1
    }

    [Tooltip("Points Prefab")]
    [SerializeField] private GameObject pointPrefab;
    [Tooltip("Indicates the type of path, if Linear or Looping")]
    [SerializeField] private PathTypes pathType;
    [Tooltip("1: Indicates clockwise/forward and -1: Indicates anticlockwise/backwards")]
    [SerializeField] private MoveDirection movementDirection = MoveDirection.Clockwise;
    [Tooltip("Indicates the point in path sequence the platform will move to")]
    [SerializeField] private int movingTo = 0;
    [Tooltip("Array containing all points in the path")]
    [SerializeField] private List<Transform> pathSequence = null;

    public void OnDrawGizmos()
    {
        if (pathSequence == null || pathSequence.Count < 2)
        {
            return; // No line creation need it
        }

        for (int i = 1; i < pathSequence.Count; i++)
        {
            Gizmos.DrawLine(pathSequence[i - 1].position, pathSequence[i].position); // drawing lines between to points
        }

        if (pathType == PathTypes.Loop)
        {
            Gizmos.DrawLine(pathSequence[0].position, pathSequence[pathSequence.Count - 1].position); // drawing lines between first point to last point
        }
    }

    public IEnumerator<Transform> GetNextPathPoint()
    {
        // Make sure that are at least two points to create a path
        if (pathSequence == null || pathSequence.Count < 1) yield break;
        
        while (true)
        {
            yield return pathSequence[movingTo];

            if (pathSequence.Count == 1) continue;

            if (pathType == PathTypes.Linear)
            {
                if (movingTo == 0)
                {
                    movementDirection = MoveDirection.Clockwise;
                }
                else if (movingTo >= pathSequence.Count - 1)
                {
                    movementDirection = MoveDirection.AntiClockwise;
                }
            }

            movingTo = movingTo + (int)movementDirection;


            if (pathType == PathTypes.Loop)
            {
                if (movingTo >= pathSequence.Count)
                {
                    movingTo = 0;
                }
                
                if (movingTo < 0)
                {
                    movingTo = pathSequence.Count - 1;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var point = Instantiate(pointPrefab, new Vector3(mp.x, mp.y), Quaternion.identity, transform);
            point.name = "p" + pathSequence.Count;
            pathSequence.Add(point.transform);

            OnPointAdded();
        }
    }
}

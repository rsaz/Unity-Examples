using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFollowPath : MonoBehaviour
{
    public enum MovementType
    {
        MoveTowards,
        LerpTowards
    }

    [Tooltip("Movement type used")]
    [SerializeField] private MovementType type = MovementType.MoveTowards;
    [Tooltip("Reference to path created. Platform Path Prefab")]
    [SerializeField] private MovementPath myPath = null;
    [Tooltip("Platform movement speed")]
    [SerializeField] private float speed = 1f;
    [Tooltip("How close the platform does it have to be to point defined")]
    [SerializeField] private float maxDistanceToPoint = .1f;

    public IEnumerator<Transform> pointsInPath;

    private void OnEnable()
    {
        MovementPath.OnPointAdded += InitPlatformFollow;
    }

    private void OnDisable()
    {
        MovementPath.OnPointAdded -= InitPlatformFollow;
    }

    private void Start()
    {
        InitPlatformFollow();
    }

    private void Update()
    {
        if (pointsInPath == null || pointsInPath.Current == null) return;
        
        if (type == MovementType.MoveTowards)
        {
            transform.position = Vector3.MoveTowards(transform.position, pointsInPath.Current.position, Time.deltaTime * speed);
        }
        else if (type == MovementType.LerpTowards)
        {
            transform.position = Vector3.Lerp(transform.position, pointsInPath.Current.position, Time.deltaTime * speed);
        }

        var distanceSQR = (transform.position - pointsInPath.Current.position).sqrMagnitude;
        if (distanceSQR < maxDistanceToPoint * maxDistanceToPoint) pointsInPath.MoveNext(); 
    }

    private void InitPlatformFollow()
    {
        if (myPath == null)
        {
            Debug.LogError("Path cannot be null. You must create a path to be followed", gameObject);
            return;
        }

        pointsInPath = myPath.GetNextPathPoint(); ;
        Debug.Log(pointsInPath.Current);

        pointsInPath.MoveNext();
        Debug.Log(pointsInPath.Current);

        if (pointsInPath.Current == null)
        {
            Debug.LogWarning("A path must contain points to follow", gameObject);
            return;
        }

        transform.position = pointsInPath.Current.position;
    }
}

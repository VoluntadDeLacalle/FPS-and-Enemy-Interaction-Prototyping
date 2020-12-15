using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavExtension;

//Gavin Wrote This

public class EnemyMovement3D : MonoBehaviour
{
    public Azee.PathFinding3D.NavGridAgent navGridAgent;

    private List<Vector3> pathToTarget = new List<Vector3>();
    private List<Vector3> currentGizmoPath;

    [Header("Nav Grid Agent Variables")]
    [Tooltip("Speed of the agent as it traverses the path.")]
    public float speed = 4f;
    [Tooltip("Will determine if the object looks along the path as it traverses.")]
    public bool lookPathDirection = false;

    [Header("Gizmo Variables")]
    [Tooltip("Determines how smooth the path of the agent will look.")]
    public float pathSegments = 1f;

    private Vector3 startPosition = Vector3.zero;
    private Vector3 endPosition = Vector3.zero;
    [HideInInspector] public Vector3 endOfPath = Vector3.zero;

    private float timeTakenDuringLerp = 0f;
    private float timeStartedLerping = 0f;
    private int count = 1;
    
    [HideInInspector] public bool isMoving = false;
    private bool isLerping = false;

    void Start()
    {
        endOfPath = transform.position;
    }

    void OnDrawGizmosSelected()
    {
        if (currentGizmoPath != null && currentGizmoPath.Count > 1)
        {
            Vector3[] smoothPath = LineSmoother.SmoothLine(currentGizmoPath.ToArray(), pathSegments);
                
            Gizmos.color = Color.green;
            for (int i = 1; i < smoothPath.Length; i++)
            {
                Gizmos.DrawLine(smoothPath[i - 1], smoothPath[i]);
            }
        }
    }

    void StartLerping()
    {
        if (count < pathToTarget.Count)
        {
            isLerping = true;
            timeStartedLerping = Time.time;

            startPosition = transform.position;
            endPosition = pathToTarget[count];

            timeTakenDuringLerp = (Vector3.Distance(startPosition, endPosition)) / speed;
        }

        isMoving = true;
    }

    public void SetDestination(Vector3 target)
    {
        pathToTarget = navGridAgent.FindPathToTarget(target);
        if (pathToTarget.Count != 0)
        {
            endOfPath = pathToTarget[pathToTarget.Count - 1];
            currentGizmoPath = new List<Vector3>(pathToTarget);

            count = 1;
            StartLerping();
        }
        else
        {
            Debug.Log("No path to target.");
        }
    }

    public void SetDestination(Transform target)
    {
        pathToTarget = navGridAgent.FindPathToTarget(target);
        if (pathToTarget.Count != 0)
        {
            endOfPath = pathToTarget[pathToTarget.Count - 1];
            currentGizmoPath = new List<Vector3>(pathToTarget);

            count = 1;
            StartLerping();
        }
        else
        {
            Debug.Log("No path to target.");
        }
    }

    public void Stop()
    {
        if (isMoving)
        {
            isLerping = false;
            isMoving = false;
            count = pathToTarget.Count;

            currentGizmoPath.Clear();
        }
    }

    void LookAtPath()
    {
        if (count > 0 && count >= pathToTarget.Count)
        {
            Vector3 direction = pathToTarget[count] - transform.position;

            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Update()
    {
        if (isLerping && lookPathDirection && pathToTarget.Count != 0)
        {
            LookAtPath();
        }
    }

    void FixedUpdate()
    {
        if (isLerping && pathToTarget.Count != 0)
        {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            transform.position = Vector3.Lerp(startPosition, endPosition, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                currentGizmoPath.Remove(pathToTarget[count]);
                isLerping = false;
                count++;

                if (currentGizmoPath.Contains(pathToTarget[0]))
                {
                    currentGizmoPath.Remove(pathToTarget[0]);
                }

                if (count != pathToTarget.Count)
                {
                    StartLerping();
                }
                else
                {
                    isMoving = false;
                    endOfPath = transform.position;
                }
            }
        }
    }
}

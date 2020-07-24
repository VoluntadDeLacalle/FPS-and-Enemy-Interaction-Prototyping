using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavExtension;

public class EnemyMovement3D : MonoBehaviour
{
    public Azee.PathFinding3D.NavGridAgent navGridAgent;

    private List<Vector3> pathToTarget = new List<Vector3>();
    private List<Vector3> currentGizmoPath;

    public float speed = 4f;
    public float pathSegments = 1f;

    private Vector3 startPosition = Vector3.zero;
    private Vector3 endPosition = Vector3.zero;

    private float timeTakenDuringLerp = 0f;
    private float timeStartedLerping = 0f;
    private int count = 1;

    public bool lookPathDirection = false;
    [HideInInspector]
    public bool isMoving = false;
    private bool isLerping = false;

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
        currentGizmoPath = new List<Vector3>(pathToTarget);

        count = 1;
        StartLerping();
    }

    public void SetDestination(Transform target)
    {
        pathToTarget = navGridAgent.FindPathToTarget(target);
        currentGizmoPath = new List<Vector3>(pathToTarget);

        count = 1;
        StartLerping();
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
        Vector3 direction = pathToTarget[count] - transform.position;

        transform.rotation = Quaternion.LookRotation(direction);
    }

    void Update()
    {
        if (isLerping && lookPathDirection)
        {
            LookAtPath();
        }
    }

    void FixedUpdate()
    {
        if (isLerping)
        {
            float timeSinceStarted = Time.time - timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

            transform.position = Vector3.Lerp(startPosition, endPosition, percentageComplete);

            if (percentageComplete >= 1.0f)
            {
                isLerping = false;
                count++;
                
                currentGizmoPath = navGridAgent.FindPathToTarget(pathToTarget[pathToTarget.Count - 1]);

                if (count != pathToTarget.Count)
                {
                    StartLerping();
                }
                else
                {
                    isMoving = false;
                }
            }
        }
    }
}

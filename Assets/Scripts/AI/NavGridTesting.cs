using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavGridTesting : MonoBehaviour
{
    public Azee.PathFinding3D.NavGridAgent navGridAgent;

    private List<Vector3> pathToTarget = new List<Vector3>();

    public float speed = 4f;

    private Vector3 startPosition = Vector3.zero;
    private Vector3 endPosition = Vector3.zero;

    private float timeTakenDuringLerp = 0f;
    private float timeStartedLerping = 0f;
    private int count = 1;

    private bool isLerping = false;

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
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pathToTarget = navGridAgent.FindPathToTarget(GameManager.instance.player.transform);

            count = 1;
            StartLerping();
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

                if (count != pathToTarget.Count)
                {
                    StartLerping();
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRaycastShoot : MonoBehaviour
{
    public float fireRate = 0;
    private float maxFireRate = 0;

    public float maxShotDistance = 0;
    [Tooltip("Determines accuracy.")]
    public float maxDeviation = 0;
    [Range(0, 360)]
    public float maxAngleOfDeviation = 0;
    
    public bool canGetAccurate = false;
    [Tooltip("Only matters if the enemy can get more accurate. Determines amount of landed shots needed to increase accuracy.")]
    public float shotsToGetAccurate = 0;
    [Tooltip("Only matters if the enemy can get more accurate. Determines what deviation is divided by to allow for more accuracy.")]
    public float accuracyIncreaseDivisor = 2;
    private float shotsLanded = 0;

    private GameObject target;
    private Vector3 positionFiredFrom = Vector3.zero;
    private Vector3 bulletHitPosition = Vector3.zero;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(positionFiredFrom, bulletHitPosition);
    }

    void Awake()
    {
        maxFireRate = fireRate;
        
        //target = GameManager.instance.player.transform.gameObject;
        positionFiredFrom = transform.position;
    }

    public void Shooting()
    {
        target = GameManager.instance.player.transform.gameObject;
        positionFiredFrom = transform.position;

        Fire();
    }

    void Fire()
    {
        fireRate -= Time.deltaTime;

        if (fireRate <= 0)
        {
            Vector3 direction = DetermineDirection();
            Ray ray = new Ray(positionFiredFrom, direction);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, maxShotDistance))
            {
                bulletHitPosition = hitInfo.point;

                if (hitInfo.transform.gameObject == target.transform.gameObject)
                {
                    shotsLanded++;

                    if (shotsLanded % shotsToGetAccurate == 0 && canGetAccurate)
                    {
                        if (maxDeviation >= 1)
                        {
                            maxDeviation /= accuracyIncreaseDivisor;
                        }
                    }
                }
            }

            Debug.Log("Fire!");
            fireRate = maxFireRate;
        }
    }
    
    Vector3 DetermineDirection()
    {
        Vector3 initialDirection = target.transform.position - positionFiredFrom;

        float deviation = Random.Range(0, maxDeviation);
        float angle = Random.Range(0, maxAngleOfDeviation);

        initialDirection = Quaternion.AngleAxis(deviation, Vector3.up) * initialDirection;
        initialDirection = Quaternion.AngleAxis(angle, Vector3.forward) * initialDirection;

        return initialDirection;
    }
}

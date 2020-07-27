using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchTimer : MonoBehaviour
{
    private float duration;
    private float range;
    private Light light;
    private float currentDuration;
    private float currentRange;
    // Start is called before the first frame update
    void Start()
    {
        light = this.gameObject.GetComponent<Light>();
    }

    private void Update()
    {
        //If a torch is on slowly fade out 
        if (currentDuration > 0)
        {
            light.enabled = true;
            light.range = currentRange;
            currentDuration -= Time.deltaTime;
            Debug.Log(currentDuration);
            currentRange = range * (currentDuration / duration);
            if (currentRange > range)
            {
                currentRange = range;
            }
        }
        else
        {
            light.enabled = false;
        }
    }

    public void Info(float time, float distance)
    {
        duration = time;
        range = distance;
        currentDuration += duration;
        currentRange = range;
    }


}

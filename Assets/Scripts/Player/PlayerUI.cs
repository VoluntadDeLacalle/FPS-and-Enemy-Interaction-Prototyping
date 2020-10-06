using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public float health;
    public float hunger;
    public float hydration;
    public float sprint;

    public float hungerDegredation;
    public float healthDegredation;
    public float hydrationDegredation;
    public float sprintDegredation;
    
    public float healthRegeneration;
    public float sprintRegeneration;

    public Text healthText;
    public Text hungerText;
    public Text hydrationText;
    public Text attackStateText;
    public Text sprintText;

    // Start is called before the first frame update
    void Start()
    {
        setStatText(health, healthText, "Health: ");
        setStatText(hunger, hungerText, "Hunger: ");
        setStatText(hydration, hydrationText, "Hydration: ");
        setStatText(sprint, sprintText, "Sprint: ");
        setattackState();
    }

    // Update is called once per frame
    void Update()
    {
        degradeHunger(hungerDegredation, healthDegredation);
        degradeHydration(hydrationDegredation, hydrationDegredation);

        setStatText(health, healthText, "Health: ");
        setStatText(hunger, hungerText, "Hunger: ");
        setStatText(hydration, hydrationText, "Hydration: ");
        setStatText(sprint, sprintText, "Sprint: ");
        setattackState();
    }


    public void setStatText(float amount, Text text, string message)
    {
        text.text = message + amount;
    }

    public void degradeHunger(float hungerDegredationRate, float healthDegredationRate)
    {
        if(hunger <= 100)
        {
            hunger -= hungerDegredationRate * Time.deltaTime;
        }

        if(hunger < 50)
        {
            health -= healthDegredationRate * Time.deltaTime;
        }
    }

    public void degradeHydration(float hydrationDegredationRate, float healthDegredationRate)
    {
        if(hydration <= 100)
        {
            hydration -= hydrationDegredationRate * Time.deltaTime;
        }

        if(hydration < 50)
        {
            health -= healthDegredationRate * Time.deltaTime;
        }
    }

    public void regenHealth(float healthRegenerationRate)
    {
        if(health < 100 && hunger > 50 && hydration > 50)
        {
            health += healthRegenerationRate * Time.deltaTime;
        }

        if(hunger < 50 || hydration < 50 || health > 100)
        {
            health += healthRegeneration * 0 * Time.deltaTime;
        }
    }

    public void setattackState()
    {
        if (FindObjectOfType<Gun>().isRaycast)
            attackStateText.text = "Shooting";

        else
            attackStateText.text = "Melee";
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController FirstPersonMovement;
    void Awake()
    {
        GameManager.instance.player = this;
        FirstPersonMovement = GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
    }
}
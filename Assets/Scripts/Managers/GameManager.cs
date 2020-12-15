using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Gavin Wrote This

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public Player player;
    public List<GremlinEnemyBehavior> gremlins;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        gremlins = new List<GremlinEnemyBehavior>();
    }
}

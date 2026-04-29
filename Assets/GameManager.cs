// GameManager.cs
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public BallController ball;
    public List<HoleController> holes;

    [Header("Spawn Settings")]
    public Transform ballSpawnPoint; 

    private int currentScore = 0;

    void Awake()
    {
        Instance = this;
    }

    
    public void OnHoleScored(HoleController hole)
    {
        currentScore++;

        bool allHolesComplete = currentScore >= holes.Count;

        if (allHolesComplete)
        {
            OnVictory();
        }
        else
        {
            
            Invoke(nameof(RespawnBall), 1.5f);
        }
    }

    
    public void OnBallMissed()
    {
        
        foreach (HoleController hole in holes)
            hole.ResetHole();

        currentScore = 0;

        RespawnBall();
    }

    void RespawnBall()
    {
        ball.ResetBall(ballSpawnPoint.position);
    }

    void OnVictory()
    {
        Debug.Log("Victory!");
    }
}
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public BallController ball;
    public List<TargetController> targets;
    public Transform ballSpawnPoint;

    private int score = 0;

    void Awake()
    {
        Instance = this;
    }

    public void OnTargetHit(TargetController target)
    {
        score++;
        Debug.Log("Target golpeado! Score: " + score);
        Invoke(nameof(RespawnBall), 1f);
    }

    public void OnBallMissed()
    {
        Debug.Log("Pelota perdida");
        Invoke(nameof(RespawnBall), 1.5f);
    }

    void RespawnBall()
    {
        if (ball != null && ballSpawnPoint != null)
            ball.ResetBall(ballSpawnPoint.position);
    }
}

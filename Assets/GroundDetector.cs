using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        
        BallController ball = collision.gameObject.GetComponent<BallController>();
        if (ball == null) return;

        GameManager.Instance.OnBallMissed();
    }
}
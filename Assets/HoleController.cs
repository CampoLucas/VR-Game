using UnityEngine;

public class HoleController : MonoBehaviour
{
    [Header("Hole Settings")]
    public Light holeLight;        
    public bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        
        if (isActivated) return;
        BallController ball = other.GetComponent<BallController>();
        if (ball == null) return;

        
        isActivated = true;

        if (holeLight != null)
            holeLight.enabled = true; 

        
        GameManager.Instance.OnHoleScored(this);

        
        other.gameObject.SetActive(false);
    }

    
    public void ResetHole()
    {
        isActivated = false;
        if (holeLight != null)
            holeLight.enabled = false;
    }
}
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BallController : MonoBehaviour
{
    [Header("Physics Settings")]
    public float gravityMultiplier = 2.5f;

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void OnReleased(SelectExitEventArgs args)
    {
        
        StartCoroutine(ApplyExtraGravity());
    }

    System.Collections.IEnumerator ApplyExtraGravity()
    {
        while (!rb.isKinematic && rb.velocity.magnitude > 0.1f)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
            yield return new WaitForFixedUpdate();
        }
    }

    public void ResetBall(Vector3 startPosition)
    {
        StopAllCoroutines();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
        gameObject.SetActive(true);
    }
}
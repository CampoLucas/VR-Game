using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public enum BallColor  { Red, Blue, Green, Yellow }
public enum BallWeight { Light, Heavy }

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class BallController : MonoBehaviour
{
    public BallColor  ballColor;
    public BallWeight ballWeight;
    public float outOfBoundsY = -1f;

    private Rigidbody          rb;
    private XRGrabInteractable grabInteractable;
    private bool               inFlight;
    private float              gravityMultiplier;

    void Awake()
    {
        rb               = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb.useGravity    = false;

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
    }

    void Start()
    {
        if (ballWeight == BallWeight.Light)
        {
            rb.mass           = 0.5f;
            gravityMultiplier = 2f;
        }
        else
        {
            rb.mass           = 2f;
            gravityMultiplier = 3.2f;
        }

        ApplyVisualColor();
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        inFlight           = false;
        rb.velocity        = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    void OnReleased(SelectExitEventArgs args)
    {
        inFlight = true;
    }

    void FixedUpdate()
    {
        if (!inFlight) return;

        rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);

        if (transform.position.y < outOfBoundsY)
        {
            inFlight = false;
            if (GameManager.Instance != null)
                GameManager.Instance.OnBallMissed();
        }
    }

    public void ResetBall(Vector3 startPosition)
    {
        inFlight           = false;
        rb.velocity        = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPosition;
        gameObject.SetActive(true);
    }

    void ApplyVisualColor()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend == null) return;

        rend.material.color = ballColor switch
        {
            BallColor.Red    => Color.red,
            BallColor.Blue   => new Color(0.2f, 0.5f, 1f),
            BallColor.Green  => new Color(0.1f, 0.8f, 0.2f),
            BallColor.Yellow => Color.yellow,
            _                => Color.white
        };
    }
}

using System.Collections;
using UnityEngine;

public enum TargetState { Idle, Moving, HitSuccess, Deactivated }

public class TargetController : MonoBehaviour
{
    [Header("Required Ball")]
    public BallColor requiredColor;
    public BallWeight requiredWeight;

    [Header("Initial State")]
    public TargetState initialState = TargetState.Idle;

    [Header("Idle — Float")]
    public float floatAmplitude = 0.06f;
    public float floatFrequency = 1.2f;

    [Header("Moving — Side to Side")]
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 1.5f;

    [Header("Hit Success")]
    public float backwardDistance = 0.5f;
    public float backwardDuration = 0.25f;
    public Light targetLight;
    public float deactivateDuration = 5f;

    private TargetState currentState;
    private Vector3 basePosition;
    private Collider col;

    void Start()
    {
        basePosition = transform.position;
        col = GetComponent<Collider>();
        currentState = initialState;
    }

    void Update()
    {
        if (currentState == TargetState.Idle)
        {
            float y = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = basePosition + Vector3.up * y;
        }
        else if (currentState == TargetState.Moving)
        {
            if (pointA != null && pointB != null)
            {
                float t = (Mathf.Sin(Time.time * moveSpeed) + 1f) * 0.5f;
                transform.position = Vector3.Lerp(pointA.position, pointB.position, t);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentState == TargetState.HitSuccess || currentState == TargetState.Deactivated) return;

        BallController ball = other.GetComponent<BallController>();
        if (ball == null) return;

        if (ball.ballColor == requiredColor && ball.ballWeight == requiredWeight)
        {
            other.gameObject.SetActive(false);
            StartCoroutine(HitSuccessRoutine());
            if (GameManager.Instance != null)
                GameManager.Instance.OnTargetHit(this);
        }
    }

    IEnumerator HitSuccessRoutine()
    {
        currentState = TargetState.HitSuccess;

        if (targetLight != null)
            targetLight.enabled = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos - transform.forward * backwardDistance;
        float elapsed = 0f;

        while (elapsed < backwardDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / backwardDuration);
            yield return null;
        }

        currentState = TargetState.Deactivated;
        SetVisible(false);
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(deactivateDuration);

        transform.position = basePosition;
        SetVisible(true);
        if (col != null) col.enabled = true;
        if (targetLight != null) targetLight.enabled = false;

        currentState = initialState;
    }

    public void ResetTarget()
    {
        StopAllCoroutines();
        transform.position = basePosition;
        SetVisible(true);
        if (col != null) col.enabled = true;
        if (targetLight != null) targetLight.enabled = false;
        currentState = initialState;
    }

    void SetVisible(bool visible)
    {
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = visible;
    }
}

using UnityEngine;
using Oculus.Interaction;

public class HandVisibilityController : MonoBehaviour
{
    public OVRGrabber leftGrabber;
    public OVRGrabber rightGrabber;
    public GameObject leftHandMesh;
    public GameObject rightHandMesh;

    void Update()
    {
        
        leftHandMesh.SetActive(leftGrabber.grabbedObject != null);
        rightHandMesh.SetActive(rightGrabber.grabbedObject != null);
    }
}
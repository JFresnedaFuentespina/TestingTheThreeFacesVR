using UnityEngine;

public class FollowOVRHands : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject leftHandEsqueleto;
    public GameObject rightHandEsqueleto;
    public GameObject leftHandVR;
    public GameObject rightHandVR;

    // Update is called once per frame
    void Update()
    {
        leftHandEsqueleto.transform.position = leftHandVR.transform.position;
        leftHandEsqueleto.transform.rotation = leftHandVR.transform.rotation;

        rightHandEsqueleto.transform.position = rightHandVR.transform.position;
        rightHandEsqueleto.transform.rotation = rightHandVR.transform.rotation;
    }

}

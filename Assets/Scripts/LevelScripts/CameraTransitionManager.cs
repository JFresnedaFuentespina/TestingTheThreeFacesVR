using System.Collections;
using UnityEngine;

public class CameraTransitionManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject camera1;
    public GameObject camera2;
    public float transitionDuration = 2.0f;

    public Vector3 camera1StartPosition;
    private Quaternion camera1StartRotation;

    public Vector3 camera2StartPosition;
    private Quaternion camera2StartRotation;

    private bool isCamera1Active = true;
    private bool isTransitioning = false;
    void Start()
    {
        camera1StartPosition = camera1.transform.position;
        camera1StartRotation = camera1.transform.rotation;

        camera2StartPosition = camera2.transform.position;
        camera2StartRotation = camera2.transform.rotation;

        isCamera1Active = true;
        camera1.SetActive(true);
        camera2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("ChangeCharacter")) && !isTransitioning)
        {
            CacheCameraPositions();
            if (isCamera1Active)
            {
                StartCoroutine(TransitionToCamera2());
            }
            else
            {
                StartCoroutine(TransitionToCamera1());
            }
        }
    }

    IEnumerator TransitionToCamera2()
    {
        GameState.isPaused = true;
        isTransitioning = true;
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            camera1.transform.position = Vector3.Lerp(camera1StartPosition, camera2.transform.position, t);
            camera1.transform.rotation = Quaternion.Slerp(camera1StartRotation, camera2.transform.rotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        camera1.transform.position = camera2.transform.position;
        camera1.transform.rotation = camera2.transform.rotation;

        camera1.SetActive(false);
        camera2.SetActive(true);

        camera1.transform.position = camera1StartPosition;
        camera1.transform.rotation = camera1StartRotation;

        isCamera1Active = false;
        isTransitioning = false;
        GameState.isPaused = false;
    }
    IEnumerator TransitionToCamera1()
    {
        GameState.isPaused = true;
        isTransitioning = true;
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            camera2.transform.position = Vector3.Lerp(camera2StartPosition, camera1.transform.position, t);
            camera2.transform.rotation = Quaternion.Slerp(camera2StartRotation, camera1.transform.rotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        camera2.transform.position = camera2StartPosition;
        camera2.transform.rotation = camera2StartRotation;

        camera2.SetActive(false);
        camera1.SetActive(true);

        camera2.transform.position = camera2StartPosition;
        camera2.transform.rotation = camera2StartRotation;

        isCamera1Active = true;
        isTransitioning = false;
        GameState.isPaused = false;
    }

    void CacheCameraPositions()
    {
        camera1StartPosition = camera1.transform.position;
        camera1StartRotation = camera1.transform.rotation;

        camera2StartPosition = camera2.transform.position;
        camera2StartRotation = camera2.transform.rotation;
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraDialogueManager : MonoBehaviour
{
    public Camera mainCamera;
    public Camera playerCamera;
    public Camera bossCamera;

    public enum DialogueCamera { Main, Player, Boss }
    public DialogueCamera cameraToUse;

    public CameraDialogueManager nextCameraDialoguemanager;

    public void RegisterPlayerCamera(Camera cam)
    {
        playerCamera = cam;

        if (nextCameraDialoguemanager != null &&
            nextCameraDialoguemanager != this)
        {
            nextCameraDialoguemanager.RegisterPlayerCamera(cam);
        }
    }

    public void RegisterBossCamera(Camera cam)
    {
        bossCamera = cam;
        if (nextCameraDialoguemanager != null &&
            nextCameraDialoguemanager != this)
        {
            nextCameraDialoguemanager.RegisterBossCamera(cam);
        }
    }

    private Coroutine cameraCoroutine;

    void OnEnable()
    {
        // Si ya hay una corrutina corriendo, cancelarla
        if (cameraCoroutine != null)
            StopCoroutine(cameraCoroutine);

        cameraCoroutine = StartCoroutine(UpdateCameraCoroutine());
    }

    IEnumerator UpdateCameraCoroutine()
    {
        // Espera mientras la cámara que necesitamos no exista
        while ((cameraToUse == DialogueCamera.Boss && bossCamera == null) ||
               (cameraToUse == DialogueCamera.Player && playerCamera == null))
        {
            yield return null; // espera un frame
        }

        // Una vez que la cámara existe, activa la correcta
        switch (cameraToUse)
        {
            case DialogueCamera.Boss:
                SetCameraDepths(bossCamera, playerCamera, mainCamera);
                Debug.Log("CDM: SHOWING BOSS");
                break;
            case DialogueCamera.Player:
                SetCameraDepths(playerCamera, bossCamera, mainCamera);
                Debug.Log("CDM: SHOWING PLAYER");
                break;
            default:
                SetCameraDepths(mainCamera, playerCamera, bossCamera);
                Debug.Log("CDM: SHOWING MAIN");
                break;
        }
    }

    // Método helper para asignar depth y clearFlags
    void SetCameraDepths(Camera camToShow, Camera cam1, Camera cam2)
    {
        if (!camToShow) return;

        camToShow.depth = 2;
        camToShow.clearFlags = CameraClearFlags.Depth;

        if (cam1) { cam1.depth = 1; cam1.clearFlags = CameraClearFlags.Depth; }
        if (cam2) { cam2.depth = 1; cam2.clearFlags = CameraClearFlags.Depth; }
    }

    public void RefreshCamera()
    {
        // Fuerza que se vuelva a aplicar el depth de la cámara correcta
        switch (cameraToUse)
        {
            case DialogueCamera.Boss:
                if (bossCamera != null) SetCameraDepths(bossCamera, playerCamera, mainCamera);
                break;
            case DialogueCamera.Player:
                if (playerCamera != null) SetCameraDepths(playerCamera, bossCamera, mainCamera);
                break;
            default:
                SetCameraDepths(mainCamera, playerCamera, bossCamera);
                break;
        }
    }


}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class MainMenuSwitch : MonoBehaviour
{
    [SerializeField] private PlayableDirector menuDirector;
    [SerializeField] private PlayableDirector sceneDirector;
    [SerializeField] private Vector3 cutsceneCameraBeginningPosition;
    [SerializeField] private Vector3 cutsceneCameraBeginningRotation;
    [SerializeField] private float transitionDuration = 2f;

    private Camera cam;
    private bool isTransitioning = false;

    private void Awake()
    {
        menuDirector.playOnAwake = true;
        sceneDirector.playOnAwake = false;

        cam = Camera.main;
    }

    public void PlayCutscene()
    {
        if (!isTransitioning) StartCoroutine(SwitchToCutscene());
    }

    private IEnumerator SwitchToCutscene()
    {
        menuDirector.Stop();
        isTransitioning = true;
        
        Vector3 startPosition = cam.transform.position;
        Quaternion startRotation = cam.transform.rotation;
        
        Quaternion targetRotation = Quaternion.Euler(cutsceneCameraBeginningRotation);
        
        float t = 0f;

        while (t < transitionDuration)
        {
            cam.transform.position = Vector3.Lerp(startPosition, cutsceneCameraBeginningPosition, t / transitionDuration);
            cam.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t / transitionDuration);
            
            t += Time.deltaTime;

            yield return null;
        }

        cam.transform.position = cutsceneCameraBeginningPosition;
        cam.transform.rotation = targetRotation;
        
        sceneDirector.Play();
        isTransitioning = false;
        
        Destroy(this);
    }
}
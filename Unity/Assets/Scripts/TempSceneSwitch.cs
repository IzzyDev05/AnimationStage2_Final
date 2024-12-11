using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSceneSwitch : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

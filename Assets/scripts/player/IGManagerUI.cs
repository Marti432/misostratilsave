using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IGManagerUI : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;
    public static bool isPaused;
    [SerializeField] bool DebugPaused;

    void Update()
    {
        DebugPaused = isPaused;
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused) Pause(true);
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused) Pause(false);
    }
    void Pause(bool state)
    {
        Time.timeScale = state ? 0 : 1;
        Debug.Log(state);
        PauseMenu.SetActive(state);
        isPaused = !isPaused;
        if (state)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void QUIT_APPLICATION()
    {
        Application.Quit();
    }
    public void RESUME()
    {
        Pause(!isPaused);
    }
}

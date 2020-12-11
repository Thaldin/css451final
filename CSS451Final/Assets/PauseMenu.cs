using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    bool IsOn = false;
    public Button quitButton = null;
    public Button resumeButton = null;
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(IsOn);
        quitButton.onClick.AddListener(Quit);
        resumeButton.onClick.AddListener(Resume);
    }
    public void Set(bool b) {
        IsOn = b;
        gameObject.SetActive(IsOn);
    }

    public void Quit() {
        Application.Quit();
    }

    public void Resume() {
        IsOn = false;
        gameObject.SetActive(IsOn);
    }
}

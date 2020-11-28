using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
    public MP5World theWorld = null;
    public CameraBehavior theCamera = null;

    private Vector3 prevMousePos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(theCamera != null, "Please set Camera Object.");
        Debug.Assert(theWorld != null, "Please set World Object.");

        // Initialized Scene
        theWorld.SetLookAtPos(Vector3.zero);
        theCamera.SetLookAt(Vector3.zero);
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    private void CheckInput()
    {
        // Alt = Camera manipulation
        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            // Mouse wheel zoom
            if (Input.mouseScrollDelta.y != 0)
            {
                theCamera.SetZoom(Input.mouseScrollDelta.y);
            }

            // Set Previous Position for calculations on MouseDown
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                prevMousePos = Input.mousePosition;
            }

            // Tumble
            if (Input.GetMouseButton(0))
            {
                var delta = prevMousePos - Input.mousePosition;
                prevMousePos = Input.mousePosition;
                theCamera.Tumble(delta);
            }

            // Slide
            if (Input.GetMouseButton(1))
            {
                var delta = prevMousePos - Input.mousePosition;
                prevMousePos = Input.mousePosition;
                theWorld.SlideLookAtPos(delta.x, delta.y);
                theCamera.Slide(delta);
                theCamera.SetLookAt(theWorld.GetLookAtPos());
            }
        }

        // Reset button
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene("ScottAndEdMP5");
        }


        // Quit
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

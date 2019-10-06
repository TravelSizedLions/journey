using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Storm;

public class Restart : MonoBehaviour
{

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            GameManager.Instance.transitions.MakeTransition("VerticalSlice", "Start");
        }
    }
}

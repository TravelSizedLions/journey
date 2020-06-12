using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.UI {
  public class ButtonIndicator : MonoBehaviour {

    /// <summary>
    /// The animator controller for the button
    /// </summary>
    private Animator Anim;


    /// <summary>
    /// The key name for the button this indicator represents.
    /// </summary>
    [SerializeField]
    private string ButtonKey = "";

    private void Awake() {
      Anim = GetComponent<Animator>();
    }


    private void Update() {
      if (Input.GetButtonDown(ButtonKey)) {
        Anim.SetTrigger("press");
      } else if (Input.GetButtonUp(ButtonKey)) {
        Anim.SetTrigger("release");
      }
    }

  }
}


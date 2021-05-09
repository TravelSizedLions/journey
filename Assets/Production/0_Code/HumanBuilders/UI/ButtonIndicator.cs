using UnityEngine;

namespace HumanBuilders {
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
      if (!PauseScreen.Paused) {
        if (Input.GetButtonDown(ButtonKey)) {
          Anim.SetBool("bool", true);
        } else if (Input.GetButtonUp(ButtonKey)) {
          Anim.SetBool("bool", false);
        }
      }
    }

  }
}


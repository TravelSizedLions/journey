using UnityEngine; 

namespace HumanBuilders {

  public class MouseIndicator : MonoBehaviour {

    /// <summary>
    /// The animator controller for the mouse
    /// </summary>
    private Animator Anim;

    /// <summary>
    /// Key for the Left Button
    /// </summary>
    [SerializeField]
    private string LeftButtonKey = "";

    /// <summary>
    /// Key for the Right Button
    /// </summary>
    [SerializeField]
    private string RightButtonKey = "";

    private void Awake() {
      Anim = GetComponent<Animator>();
    }

    private void Update() {
      if (!PauseScreen.Paused) {
        if (Input.GetButton(LeftButtonKey)) {
          Anim.SetBool("left", true);
        } else if (Anim.GetBool("left")) {
          Anim.SetBool("left", false);
        } 

        if (Input.GetButton(RightButtonKey)) {
          Anim.SetBool("right", true);
        } else if (Anim.GetBool("right")) {
          Anim.SetBool("right", false);
        }
      }
    }

  }
}
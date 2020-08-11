using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.Drawers;
#endif

namespace Storm.UI {
  public class MainMenuButton : Button {

    /// <summary>
    /// The animator controller for the button.
    /// </summary>
    public Animator anim;

    private new void Awake() {
      base.Awake();
      anim = GetComponentInChildren<Animator>();
    }

    public override void OnSelect(BaseEventData eventData) {
      base.OnSelect(eventData);
      anim.SetTrigger("select");
    }


    public override void OnDeselect(BaseEventData eventData) {
      base.OnDeselect(eventData);
      anim.SetTrigger("deselect");
    }
  }
}
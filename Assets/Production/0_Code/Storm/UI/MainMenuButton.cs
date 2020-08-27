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

    public EventSystem events;

    public MainMenu menu;

    private new void Awake() {
      base.Awake();
      anim = GetComponentInChildren<Animator>();
      events = GameObject.FindObjectOfType<EventSystem>();
      menu = GameObject.FindObjectOfType<MainMenu>();
    }

    public override void OnSelect(BaseEventData eventData) {
      base.OnSelect(eventData);
      anim.SetTrigger("select");
      menu.CurrentButton = this;
    }


    public override void OnDeselect(BaseEventData eventData) {
      base.OnDeselect(eventData);
      anim.SetTrigger("deselect");
    }

    public override void OnPointerEnter(PointerEventData eventData) {
      base.OnPointerEnter(eventData);
      EventSystem.current.SetSelectedGameObject(null);

      OnSelect(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData) {
      base.OnPointerExit(eventData);
      OnDeselect(eventData);
    }
  }
}
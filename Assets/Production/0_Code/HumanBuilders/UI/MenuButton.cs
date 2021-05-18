using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor.Drawers;
#endif

namespace HumanBuilders {
  [RequireComponent(typeof(ButtonSettings))]
  public class MenuButton : Button {

    /// <summary>
    /// The animator controller for the button.
    /// </summary>
    public Animator anim;

    /// <summary>
    /// Some tasty events.
    /// </summary>
    public EventSystem events;

    /// <summary>
    /// A reference to the main menu script.
    /// </summary>
    public MainMenu menu;

    /// <summary>
    /// Some additional settings for this button (since for some stupid reason
    /// Unity won't serialize public fields for subclasses of <see cref="Button"/>).
    /// </summary>
    private ButtonSettings settings;

    private new void Awake() {
      base.Awake();
      anim = GetComponentInChildren<Animator>();
      events = GameObject.FindObjectOfType<EventSystem>();
      menu = GameObject.FindObjectOfType<MainMenu>();
      settings = GetComponent<ButtonSettings>();
    }

    public override void OnSelect(BaseEventData eventData) {
      base.OnSelect(eventData);
      anim.SetTrigger("select");
      if (menu != null) {
        menu.CurrentButton = this;
      }
    }

    public override void OnDeselect(BaseEventData eventData) {
      base.OnDeselect(eventData);
      anim.SetTrigger("deselect");
    }

    public override void OnPointerEnter(PointerEventData eventData) {
      base.OnPointerEnter(eventData);
      EventSystem.current.SetSelectedGameObject(null);

      if (settings != null) {
        AudioManager.Play(settings.HoverSound);
      }
      
      OnSelect(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData) {
      base.OnPointerExit(eventData);
      OnDeselect(eventData);
    }
  }
}
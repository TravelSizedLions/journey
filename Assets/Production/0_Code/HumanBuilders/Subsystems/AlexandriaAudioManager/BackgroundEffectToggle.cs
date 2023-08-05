using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class BackgroundEffectToggle : MonoBehaviour, IToggleable {

    [SerializeField]
    [ValueDropdown("GetLibraries")]
    protected SoundLibrary library;

    [SerializeField]
    [ValueDropdown("GetSounds")]
    protected string sound;

    [SerializeField]
    [ReadOnly]
    private bool playing;

    public bool Toggle() {
      if (playing) {
        TurnOff();
      } else {
        TurnOn();
      }

      return playing;
    }

    public void TurnOff() {
      Sound s = library?[sound];
      if (s != null) {
        AlexandriaAudioManager.RemoveBackgroundEffect(s);
        playing = false;
      }
    }

    public void TurnOn() {
      Sound s = library?[sound];
      if (s != null) {
        AlexandriaAudioManager.AddBackgroundEffect(s);
        playing = true;
      }
    }

    private IEnumerable<SoundLibrary> GetLibraries() => AlexandriaMasterLibrary.Get().Libraries;
    private IEnumerable<string> GetSounds() => AlexandriaUtils.GetSounds(library);
  }
}
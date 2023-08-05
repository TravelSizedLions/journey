using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class SoundTrigger : MonoBehaviour, ITriggerable {

    [SerializeField]
    [ValueDropdown("GetLibraries")]
    protected SoundLibrary library;

    [SerializeField]
    [ValueDropdown("GetSounds")]
    protected string sound;

    public virtual void Pull() {
      Sound s = library?[sound];
      if (s != null) {
        AlexandriaAudioManager.PlaySound(s);
      }
    }

    private IEnumerable<SoundLibrary> GetLibraries() => AlexandriaMasterLibrary.Get().Libraries;
    private IEnumerable<string> GetSounds() => AlexandriaUtils.GetSounds(library);
  }
}
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

    private IEnumerable<SoundLibrary> GetLibraries() {
      return AlexandriaMasterLibrary.Get().Libraries;
    }

    private IEnumerable<string> GetSounds() {
      if (library != null) {
        List<string> soundNames = new List<string>();
        library.Sounds.ForEach((Sound sound) => {
          soundNames.Add(sound.Clip.name);
        });

        return soundNames;
      }

      return null;
    }
  }
}
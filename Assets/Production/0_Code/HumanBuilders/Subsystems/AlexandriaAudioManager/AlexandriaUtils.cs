
using System.Collections.Generic;

namespace HumanBuilders {
  public static class AlexandriaUtils {

    public static IEnumerable<string> GetSounds(SoundLibrary lib) {
      if (lib != null) {
        List<string> soundNames = new List<string>();
        lib.Sounds.ForEach((Sound sound) => {
          soundNames.Add(sound.Clip.name);
        });

        return soundNames;
      }

      return null;
    }
  }
}
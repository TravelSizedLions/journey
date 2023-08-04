namespace HumanBuilders {
  public class MusicTrigger : SoundTrigger {

    public override void Pull() {
      Sound s = library?[sound];
      if (s != null) {
        AlexandriaAudioManager.PlayMusic(s);
      }
    }
  }
}
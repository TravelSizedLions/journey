using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class AlexandriaAudioManager : Singleton<AlexandriaAudioManager> {

    public AudioSource MusicSource {
      get {
        musicSource = musicSource ?? CreateMusicSource();
        return musicSource;
      }
    }

    public AudioSource EffectsSource {
      get {
        effectsSource = effectsSource ?? CreateEffectsSource();
        return effectsSource;
      }
    }
    private AudioSource musicSource;
    private AudioSource effectsSource;

    private AudioSource CreateMusicSource() {
      var source = (new GameObject("music-source")).AddComponent<AudioSource>();
      source.transform.SetParent(transform);
      return source;
    }

    private AudioSource CreateEffectsSource() {
      var source = (new GameObject("effects-source")).AddComponent<AudioSource>();
      source.transform.SetParent(transform);
      return source;
    }
    
    public static void PlaySound(Sound sound) => Instance.PlaySound_Inner(sound);
    private void PlaySound_Inner(Sound sound) {
      EffectsSource.PlayOneShot(sound.Clip);
    }

    public static void PlayMusic(Sound music) => Instance.PlayMusic_Inner(music);
    private void PlayMusic_Inner(Sound music) {
      if (MusicSource.clip != music.Clip) {
        MusicSource.clip = music.Clip;
        MusicSource.loop = music.Loop;
        MusicSource.Play();
      }
    }
  }
}
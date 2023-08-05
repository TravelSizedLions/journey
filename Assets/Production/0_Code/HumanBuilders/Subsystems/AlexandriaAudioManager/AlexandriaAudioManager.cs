using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace HumanBuilders {
  public class AlexandriaAudioManager : Singleton<AlexandriaAudioManager> {

    public AudioSource MusicSource {
      get {
        if (musicSource == null) {
          musicSource = CreateMusicSource();
        }
        return musicSource;
      }
    }

    public AudioSource EffectsSource {
      get {
        if (effectsSource == null) {
          effectsSource = CreateEffectsSource();
        }
        return effectsSource;
      }
    }
    private AudioSource musicSource;
    private AudioSource effectsSource;

    private AudioSource CreateMusicSource() {
      var source = (new GameObject("music-source")).AddComponent<AudioSource>();
      var mixer = Resources.Load<AudioMixer>("master-mixer");
      var groups = mixer.FindMatchingGroups("Master/Music");
      if (groups.Length == 0) {
        Debug.LogError("Could not find music audio group");
      }

      source.outputAudioMixerGroup = groups[0];
      source.transform.SetParent(transform);
      return source;
    }

    private AudioSource CreateEffectsSource() {
      var source = (new GameObject("effects-source")).AddComponent<AudioSource>();
      var mixer = Resources.Load<AudioMixer>("master-mixer");
      var groups = mixer.FindMatchingGroups("Master/SFX");
      if (groups.Length == 0) {
        Debug.LogError("Could not find sfx audio group");
      }

      source.outputAudioMixerGroup = groups[0];
      source.transform.SetParent(transform);
      return source;
    }
    
    public static void PlaySound(Sound sound) => Instance.PlaySound_Inner(sound);
    private void PlaySound_Inner(Sound sound) {
      EffectsSource.volume = sound.Volume;
      EffectsSource.PlayOneShot(sound.Clip);
    }

    public static void PlayMusic(Sound music) => Instance.PlayMusic_Inner(music);
    private void PlayMusic_Inner(Sound music) {
      Debug.Log(string.Format("starting music with vol: {0}", music.Volume));
      if (MusicSource.clip != music.Clip) {
        MusicSource.clip = music.Clip;
        MusicSource.loop = music.Loop;
        MusicSource.Play();
      }
    }
  }
}
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace HumanBuilders {
  public class AlexandriaAudioManager : Singleton<AlexandriaAudioManager> {

    public static Dictionary<string, AudioSource> BackgroundSources {
      get {
        if (Instance.backgroundSources == null) {
          Instance.backgroundSources = new Dictionary<string, AudioSource>();
        }

        return Instance.backgroundSources;
      }
    }

    public static AudioSource MusicSource {
      get {
        if (Instance.musicSource == null) {
          Instance.musicSource = Instance.CreateMusicSource();
        }
        return Instance.musicSource;
      }
    }

    public static AudioSource EffectsSource {
      get {
        if (Instance.effectsSource == null) {
          Instance.effectsSource = Instance.CreateEffectsSource();
        }
        return Instance.effectsSource;
      }
    }

    private Dictionary<string, AudioSource> backgroundSources;
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

    private AudioSource CreateBackgroundAudioSource(Sound s) {
      if (BackgroundSources.ContainsKey(s.Name)) {
        return BackgroundSources[s.Name];
      }

      string name = string.Format("effects-source-{0}", s.Name.ToLower());
      var source = (new GameObject(name)).AddComponent<AudioSource>();
      var mixer = Resources.Load<AudioMixer>("master-mixer");
      var groups = mixer.FindMatchingGroups("Master/BackgroundFX");
      if (groups.Length == 0) {
        Debug.LogError("Could not find background effects audio group");
      }

      source.outputAudioMixerGroup = groups[0];
      source.transform.SetParent(transform);

      BackgroundSources.Add(s.Name, source);
      return source;
    }

    private void DestroyBackgroundAudioSource(Sound s) {
      if (BackgroundSources.ContainsKey(s.Name)) {
        var go = BackgroundSources[s.Name].gameObject;
        BackgroundSources.Remove(s.Name);
        Destroy(go);
      }

      Debug.LogWarning(string.Format("could not find audio source for {0}", s.Name));
    }
    
    public static void PlaySound(Sound sound) => Instance.PlaySound_Inner(sound);
    private void PlaySound_Inner(Sound sound) {
      EffectsSource.volume = sound.Volume;
      EffectsSource.loop = sound.Loop;
      EffectsSource.PlayOneShot(sound.Clip);
    }

    public static void PlayMusic(Sound music) => Instance.PlayMusic_Inner(music);
    private void PlayMusic_Inner(Sound music) {
      if (MusicSource.clip != music.Clip) {
        MusicSource.clip = music.Clip;
        MusicSource.loop = music.Loop;
        MusicSource.volume = music.Volume;
        MusicSource.Play();
      }
    }

    public static void StopMusic() => Instance.StopMusic_Inner();
    private void StopMusic_Inner() {
      MusicSource.Stop();
    }


    public static void AddBackgroundEffect(Sound s) => Instance.AddBackgroundEffect_Inner(s);

    private void AddBackgroundEffect_Inner(Sound s) {
      var source = CreateBackgroundAudioSource(s);
      source.clip = s.Clip;
      source.volume = s.Volume;
      source.loop = s.Loop;
      source.Play();
    }


    public static void RemoveBackgroundEffect(Sound s) => Instance.DestroyBackgroundAudioSource(s);
  }
}
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace HumanBuilders {

  [CreateAssetMenu(fileName="sound_settings", menuName="Journey/Sound Settings")]
  public class SoundSettings : ScriptableObject {

    [Tooltip("The audio mixer object that controls all audio levels for the game.")]
    [PropertyOrder(0)]
    [Space(10)]
    public AudioMixer Mixer;
    public AudioMixerGroup SFXGroup;

    public static SoundSettings GetSettings() {
      return Resources.Load<SoundSettings>("sound_settings");
    }

    public float MasterVolume {
      get { 
        if (VSave.GetGeneral<float>("sound_settings", "volume_master", out float val)) {
          masterVolume = val;
        } else {
          masterVolume = DefaultMasterVolume;
          VSave.SetGeneral<float>("sound_settings", "volume_master", masterVolume);
          VSave.SaveGeneral();
        }
        return masterVolume; 
      }
      set {
        masterVolume = value;
        VSave.SetGeneral<float>("sound_settings", "volume_master", masterVolume);
        VSave.SaveGeneral();
        SetMasterVolume();
      }
    }


    public float MusicVolume {
      get {
        if (VSave.GetGeneral<float>("sound_settings", "volume_music", out float val)) {
          musicVolume = val;
        } else {
          musicVolume = DefaultMusicVolume;
          VSave.SetGeneral<float>("sound_settings", "volume_music", musicVolume);
          VSave.SaveGeneral();
        }
        return musicVolume;
      }
      set {
        musicVolume = value;
        VSave.SetGeneral<float>("sound_settings", "volume_music", musicVolume);
        VSave.SaveGeneral();
        SetMusicVolume();
      }
    }

    public float SFXVolume {
      get {
        if (VSave.GetGeneral<float>("sound_settings", "volume_sfx", out float val)) {
          sfxVolume = val;
        } else {
          sfxVolume = DefaultSFXVolume;
          VSave.SetGeneral<float>("sound_settings", "volume_sfx", sfxVolume);
          VSave.SaveGeneral();
        }
        return sfxVolume;
      }
      set { 
        sfxVolume = value;
        VSave.SetGeneral<float>("sound_settings", "volume_sfx", sfxVolume);
        VSave.SaveGeneral();
        SetSFXVolume();
      }
    }

    public float BackgroundFXVolume {
      get {
        if (VSave.GetGeneral<float>("sound_settings", "volume_bgfx", out float val)) {
          bgfxVolume = val;
        } else {
          bgfxVolume = DefaultBGFXVolume;
          VSave.SetGeneral<float>("sound_settings", "volume_bgfx", bgfxVolume);
          VSave.SaveGeneral();
        }
        return bgfxVolume;
      }
      set { 
        bgfxVolume = value;
        VSave.SetGeneral<float>("sound_settings", "volume_bgfx", bgfxVolume);
        VSave.SaveGeneral();
        SetBackgroundFXVolume();
      }
    }

    [SerializeField]
    [Range(0, 1)]
    [BoxGroup("Current Settings")]
    [OnValueChanged("SetMasterVolume")]
    private float masterVolume = 0.5f;

    [SerializeField]
    [Range(0, 1)]
    [BoxGroup("Current Settings")]
    [OnValueChanged("SetMusicVolume")]
    private float musicVolume = 0.5f;

    [SerializeField]
    [Range(0, 1)]
    [BoxGroup("Current Settings")]
    [OnValueChanged("SetSFXVolume")]
    private float sfxVolume = 0.5f;

    [SerializeField]
    [Range(0, 1)]
    [BoxGroup("Current Settings")]
    [OnValueChanged("SetBackgroundFXVolume")]
    private float bgfxVolume = 0.5f;


    [Range(0,1)]
    [BoxGroup("Default Settings")]
    public float DefaultMasterVolume = 0.5f;

    [Range(0,1)]
    [BoxGroup("Default Settings")]
    public float DefaultMusicVolume = 0.5f;

    [Range(0,1)]
    [BoxGroup("Default Settings")]
    public float DefaultSFXVolume = 0.5f;

    [Range(0,1)]
    [BoxGroup("Default Settings")]
    public float DefaultBGFXVolume = 0.5f;

    [Button(ButtonSizes.Medium)]
    public void ResetDefaults() {
      masterVolume = DefaultMasterVolume;
      musicVolume = DefaultMusicVolume;
      sfxVolume = DefaultSFXVolume;
      bgfxVolume = DefaultBGFXVolume;
    }

    [Button(ButtonSizes.Medium)]
    public void ClearSavedValues() {
      VSave.ClearGeneral<float>("sound_settings", "volume_master");
      VSave.ClearGeneral<float>("sound_settings", "volume_music");
      VSave.ClearGeneral<float>("sound_settings", "volume_sfx");
    }

    public void InitializeMixer() {
      masterVolume = VSave.GetGeneral<float>("sound_settings", "volume_master");
      musicVolume = VSave.GetGeneral<float>("sound_settings", "volume_music");
      sfxVolume = VSave.GetGeneral<float>("sound_settings", "volume_sfx");

      SetMasterVolume();
      SetMusicVolume();
      SetSFXVolume();
    }

    public void SetMasterVolume() {
      if (Mixer != null) {
        Mixer.SetFloat("volume_master", Mathf.Log10(masterVolume)*20);
      }
    }

    public void SetMusicVolume() {
      if (Mixer != null) {
        Mixer.SetFloat("volume_music", Mathf.Log10(musicVolume)*20);
      }
    }

    public void SetSFXVolume() {
      if (Mixer != null) {
        Mixer.SetFloat("volume_sfx", Mathf.Log10(sfxVolume)*20);
      }
    }

    public void SetBackgroundFXVolume() {
      if (Mixer != null) {
        Mixer.SetFloat("volume_bgfx", Mathf.Log10(bgfxVolume)*20);
      }
    }
  }
}

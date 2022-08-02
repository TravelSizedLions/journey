using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace HumanBuilders {

  [CreateAssetMenu(fileName="sound_settings", menuName="Journey/Sound Settings")]
  public class SoundSettings : ScriptableObject {
    //-------------------------------------------------------------------------
    // Editor-Only Stuff
    //-------------------------------------------------------------------------
    #if UNITY_EDITOR

    [Tooltip("The audio mixer object that controls all audio levels for the game.")]
    [PropertyOrder(0)]
    [Space(10)]
    public AudioMixer AudioMixer;
    #endif

    public static SoundSettings GetSettings() {
      return Resources.Load<SoundSettings>("sound_settings");
    }

    public float MasterVolume {
      get { return masterVolume; }
      set {
        masterVolume = value;
        SetMasterVolume();
      }
    }


    public float MusicVolume {
      get { return musicVolume; }
      set {
        musicVolume = value;
        SetMusicVolume();
      }
    }

    public float SFXVolume {
      get { return sfxVolume; }
      set { 
        sfxVolume = value;
        SetSFXVolume();
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


    [Range(0,1)]
    [BoxGroup("Default Settings")]
    public float DefaultMasterVolume = 0.5f;

    [Range(0,1)]
    [BoxGroup("Default Settings")]
    public float DefaultMusicVolume = 0.5f;

    [Range(0,1)]
    [BoxGroup("Default Settings")]
    public float DefaultSFXVolume = 0.5f;


    [Button(ButtonSizes.Medium)]
    public void ResetDefaults() {
      MasterVolume = DefaultMasterVolume;
      MusicVolume = DefaultMusicVolume;
      SFXVolume = DefaultSFXVolume;
    }

    public void InitializeMixer() {
      SetMasterVolume();
      SetMusicVolume();
      SetSFXVolume();
    }

    public void SetMasterVolume() {
      if (AudioMixer != null) {
        Debug.Log("Master:"+masterVolume);
        AudioMixer.SetFloat("volume_master", Mathf.Log10(masterVolume)*20);
        VSave
      }
    }

    public void SetMusicVolume() {
      if (AudioMixer != null) {
        Debug.Log("Music:"+musicVolume);
        AudioMixer.SetFloat("volume_music", Mathf.Log10(musicVolume)*20);
      }
    }

    public void SetSFXVolume() {
      if (AudioMixer != null) {
        Debug.Log("SFX:"+sfxVolume);
        AudioMixer.SetFloat("volume_sfx", Mathf.Log10(sfxVolume)*20);
      }
    }
  }
}

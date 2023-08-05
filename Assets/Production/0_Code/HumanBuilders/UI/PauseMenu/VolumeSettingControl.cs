using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Reflection;

namespace HumanBuilders {
  public class VolumeSettingControl: MonoBehaviour {
    
    [SerializeField]
    private TextMeshProUGUI displayNameGUI;

    [SerializeField]
    private Slider volumeSliderGUI;

    [SerializeField]
    private TextMeshProUGUI volumeAmountGUI;

    private string settingFile;
    private string settingKey;

    public void LoadSetting(string file, string key) {
      settingKey = key;
      settingFile = file;
    
      UnityEngine.Object obj = Resources.Load(file);

      Type t = obj.GetType();
      PropertyInfo prop = t.GetProperty(key);
      float value = (float)prop.GetValue(obj);

      SetVolumeDisplay(value*10);
      SetVolumeSlider(value);
    }

    public void SetDisplayName(string displayName) {
      if (displayNameGUI != null) {
        displayNameGUI.text = displayName;
      } else {
        Debug.LogWarning(string.Format("Display Name TextMesh is null for setting \"{0}\"", displayName));
      }
    }

    public void SetVolumeDisplay(float value) {
      if (volumeAmountGUI != null) {
        volumeAmountGUI.text = ""+Mathf.RoundToInt(value*10);
      } else {
        Debug.LogWarning("Volume Display TextMesh is null");
      }
    }

    public void SetVolumeSlider(float value) {
      if(volumeSliderGUI != null) {
        volumeSliderGUI.value = value*10;
      }
    }

    public void SetVolume(float value) {
      SetVolumeDisplay(value);
      UnityEngine.Object obj = Resources.Load(settingFile);

      Type t = obj.GetType();
      PropertyInfo prop = t.GetProperty(settingKey);
      prop.SetValue(obj, value/10);
    }
  }
}
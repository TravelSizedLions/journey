using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace HumanBuilders {
  public class VolumeSettingControl: MonoBehaviour {
    
    [SerializeField]
    private TextMeshProUGUI displayNameGUI;

    [SerializeField]
    private Slider volumeSliderGUI;

    [SerializeField]
    private TextMeshProUGUI volumeAmountGUI;

    private string settingKey;


    public void LoadSetting(string key) {
      settingKey = key;
    }

    public void SetDisplayName(string displayName) {
      if (displayNameGUI != null) {
        displayNameGUI.text = displayName;
      } else {
        Debug.LogWarning(string.Format("Display Name TextMesh is null for setting \"{0}\"", displayName));
      }
    }

    public void SetVolume(float value) {
      if (volumeAmountGUI != null) {
        volumeAmountGUI.text = ""+(value*10);
      } else {
        Debug.LogWarning("Volume Display TextMesh is null");
      }
    }
  }
}
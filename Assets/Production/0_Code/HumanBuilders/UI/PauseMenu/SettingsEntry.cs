using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  [Serializable]
  public class SettingsEntry {
    public string SettingName;
    public string SettingFile;
    public string DisplayName;
    public GameObject Prefab;
  }
}
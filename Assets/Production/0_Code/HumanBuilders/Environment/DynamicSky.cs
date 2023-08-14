using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.Universal;

namespace HumanBuilders {
  public class DynamicSky : Singleton<DynamicSky> {
    
    [Range(0, 1)]
    [FoldoutGroup("Brightness")]
    [Tooltip("The lighting intensity for daytime.")]
    public float DayBrightness;

    [Range(0, 1)]
    [Tooltip("The lighting intensity for nighttime.")]
    [FoldoutGroup("Brightness")]
    public float NightBrightness;

    [Tooltip("The lighting color for nighttime.")]
    [FoldoutGroup("Color")]
    public Color DayColor;

    [Tooltip("The lighting color for nighttime.")]
    [FoldoutGroup("Color")]
    public Color NightColor;

    [SerializeField]
    [ReadOnly]
    private bool isNighttime;

    private void OnDestroy() {
      var children = new List<GameObject>();

      foreach (var transform in GetComponentsInChildren<Transform>(true)) {
        children.Add(transform.gameObject);
      }

      children.ForEach(child => Destroy(child));
    }

    public void SetDay() {
      GlobalLight.Settings.intensity = DayBrightness;
      GlobalLight.Settings.color = DayColor;
    }

    public void SetNight() {
      GlobalLight.Settings.intensity = NightBrightness;
      GlobalLight.Settings.color = NightColor;
    }
  }
}
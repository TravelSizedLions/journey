using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Rendering.Universal;

namespace HumanBuilders {
  public class DynamicSky : Singleton<DynamicSky> {
    
    [Range(0, 1)]
    [Tooltip("The lighting intensity for daytime.")]
    public float DayBrightness;

    [Range(0, 1)]
    [Tooltip("The lighting intensity for nighttime.")]
    public float NightBrightness;

    [SerializeField]
    [ReadOnly]
    private bool isNighttime;

    public Light2D GlobalLight => (globalLight = globalLight ?? GetComponentInChildren<Light2D>());
    private Light2D globalLight;

    private void OnDestroy() {
      var children = new List<GameObject>();

      foreach (var transform in GetComponentsInChildren<Transform>(true)) {
        children.Add(transform.gameObject);
      }

      children.ForEach(child => Destroy(child));
    }

    public void SetDay() {
      GlobalLight.intensity = DayBrightness;
    }

    public void SetNight() {
      GlobalLight.intensity = NightBrightness;
    }
  }
}
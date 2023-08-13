using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HumanBuilders {
  public class GlobalLight: Singleton<GlobalLight> {
    public static Light2D Settings {
      get {
        if (Instance.settings == null) {
          Instance.settings = Instance.gameObject.AddComponent<Light2D>();
          Instance.settings.lightType = Light2D.LightType.Global;
          Instance.settings.intensity = .1f;
        }

        return Instance.settings;
      }
    }

    [SerializeField]
    [ReadOnly]
    [InfoBox("Unity throws errors if heaven forbid there's more than one global light in a scene. So, instead of having a light already in the prefab, we create a singleton and build the light only after the singleton culls other references to itself.", InfoMessageType.Warning)]
    private Light2D settings;
  }
}
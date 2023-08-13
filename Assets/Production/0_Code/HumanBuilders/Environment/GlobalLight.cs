using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HumanBuilders {
  public class GlobalLight: Singleton<GlobalLight> {

    public static Light2D Settings => (Instance.settings = Instance.settings ?? Instance.GetComponent<Light2D>());
    private Light2D settings;
  }
}
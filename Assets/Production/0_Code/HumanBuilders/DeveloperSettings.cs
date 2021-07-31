using UnityEngine;

namespace HumanBuilders {

  [CreateAssetMenu(fileName="dev_settings", menuName="Journey/Developer Settings")]
  public class DeveloperSettings : ScriptableObject {
    public bool SkipBossBattles;

    public static DeveloperSettings GetSettings() {
      return Resources.Load<DeveloperSettings>("dev_settings");
    }
  }
}
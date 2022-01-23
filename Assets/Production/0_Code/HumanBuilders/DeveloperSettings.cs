using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
namespace HumanBuilders {

  [CreateAssetMenu(fileName="dev_settings", menuName="Journey/Developer Settings")]
  public class DeveloperSettings : ScriptableObject {
    public static DeveloperSettings GetSettings() {
      return Resources.Load<DeveloperSettings>("dev_settings");
    }

    [Tooltip("The  variable that controls whether or not the player's companion is following the player.")]
    public Variable CompanionFollowingVariable;

    [Tooltip("The  variable that controls whether or not the player's companion is active and visible.")]
    public Variable CompanionActiveVariable;

    //-------------------------------------------------------------------------
    // Editor-Only Stuff
    //-------------------------------------------------------------------------
    #if UNITY_EDITOR
    public bool SkipBossBattles;

    [Tooltip("The path to the player companion's animator controller.")]
    public AnimatorController CompanionAnimatorController;
    #endif
  }
}

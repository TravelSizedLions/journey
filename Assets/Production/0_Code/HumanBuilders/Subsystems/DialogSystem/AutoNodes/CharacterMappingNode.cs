using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  [NodeWidth(NodeWidths.WIDE)]
  [NodeTint(NodeColors.BASIC_COLOR)]
  [CreateNodeMenu("Dialog/Character Mapping")]
  public class CharacterMappingNode : AutoNode {
    private const string IDLE_TRIGGER = "idle";
    private const string TALKING_TRIGGER = "talking";

    [AutoTable(typeof(CharacterMapping), "Characters", NodeColors.BASIC_COLOR)]
    public List<CharacterMapping> Mapping;

    [Space(5)]
    [LabelWidth(150)]
    public CharacterProfile PlayerCharacterProfile;

    private Dictionary<string, Animator> MapCache; 

    public void StartTalking(string characterName) {
      Debug.Log("Start talking: " + characterName);
      if (MapCache.ContainsKey(characterName)) {
        Debug.Log("Found character");
        Animator anim = MapCache[characterName];
        if (AnimationTools.HasParameter(anim, TALKING_TRIGGER) && AnimationTools.HasParameter(anim, IDLE_TRIGGER)) {
          Debug.Log("Triggering");
          anim.ResetTrigger(IDLE_TRIGGER);
          anim.SetTrigger(TALKING_TRIGGER);
        }
      }
    }

    public void StopTalking(string characterName) {
      Debug.Log("Stop talking: " + characterName);
      if (MapCache.ContainsKey(characterName)) {
        Animator anim = MapCache[characterName];
        if (AnimationTools.HasParameter(anim, IDLE_TRIGGER)) {
          anim.SetTrigger(IDLE_TRIGGER);
        }
      }
    }

    public void StopAllTalking() {
      foreach (string character in MapCache.Keys) {
        StopTalking(character);
      }
    }

    protected override void Init() {
      MapCache = new Dictionary<string, Animator>();
      foreach (CharacterMapping pair in Mapping) {
        Debug.Log("Adding: " + pair.Profile.CharacterName);
        MapCache.Add(pair.Profile.CharacterName, pair.Actor);
      }

      if (PlayerCharacterProfile != null) {
        MapCache.Add(PlayerCharacterProfile.CharacterName, GameManager.Player.Animator);
      }
    }
  }
}
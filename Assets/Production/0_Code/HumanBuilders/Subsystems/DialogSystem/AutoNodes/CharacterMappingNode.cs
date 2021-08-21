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
    private const string PROTAGONIST_PROFILE = "MainCharacters/Jerrod";

    [AutoTable(typeof(CharacterMapping), "Characters", NodeColors.BASIC_COLOR)]
    public List<CharacterMapping> Mapping;

    private Dictionary<string, Animator> MapCache; 

    public void StartTalking(string characterName) {
      if (MapCache.ContainsKey(characterName)) {
        Animator anim = MapCache[characterName];
        if (AnimationTools.HasParameter(anim, TALKING_TRIGGER) && AnimationTools.HasParameter(anim, IDLE_TRIGGER)) {
          anim.ResetTrigger(IDLE_TRIGGER);
          anim.SetTrigger(TALKING_TRIGGER);
        }
      }
    }

    public void StopTalking(string characterName) {
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
      if (Mapping != null) {
        foreach (CharacterMapping pair in Mapping) {
          MapCache.Add(pair.Profile.CharacterName, pair.Actor);
        }
      }

      if (Application.isPlaying) {
        MapCache.Add(Resources.Load<CharacterProfile>(PROTAGONIST_PROFILE).CharacterName, GameManager.Player.Animator);
      }
    }
  }
}
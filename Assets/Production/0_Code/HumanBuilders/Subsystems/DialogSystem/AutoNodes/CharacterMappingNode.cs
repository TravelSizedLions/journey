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

    public void StartTalking(CharacterProfile character) {
      string key = GetCharacterKey(character);
      if (MapCache.ContainsKey(key)) {
        Animator anim = MapCache[key];
        if (AnimationTools.HasParameter(anim, TALKING_TRIGGER) && AnimationTools.HasParameter(anim, IDLE_TRIGGER)) {
          anim.ResetTrigger(IDLE_TRIGGER);
          anim.SetTrigger(TALKING_TRIGGER);
        }
      }
    }

    public void StopTalking(CharacterProfile character) {
      string key = GetCharacterKey(character);
      StopTalking(key);
    }

    private void StopTalking(string characterKey) {
      if (MapCache.ContainsKey(characterKey)) {
        Animator anim = MapCache[characterKey];
        if (AnimationTools.HasParameter(anim, IDLE_TRIGGER)) {
          anim.SetTrigger(IDLE_TRIGGER);
        }
      }
    }

    public void StopAllTalking() {
      foreach (string characterKey in MapCache.Keys) {
        StopTalking(characterKey);
      }
    }

    protected override void Init() {
      MapCache = new Dictionary<string, Animator>();
      string key;
      if (Mapping != null) {
        foreach (CharacterMapping pair in Mapping) {
          if (pair != null && pair.Profile != null && pair.Actor != null) {
            key = GetCharacterKey(pair.Profile);
            MapCache.Add(key, pair.Actor);
          } else {
            if (pair == null) {
              Debug.LogError("Empty character mapping pair in graph: \""+graph.name+"\"");
            } else {
              if (pair.Profile == null && pair.Actor != null) {
                Debug.LogError("Empty character profile for actor \"" + pair.Actor.gameObject.name + "\"");
              } else if (pair.Actor == null && pair.Profile != null) {
                key = GetCharacterKey(pair.Profile);
                MapCache.Add(key, GameManager.Player.Animator);
              } else {
                Debug.LogError("Empty character mapping pair in graph: \""+graph.name+"\"");
              }
            }
          }
        }
      }

      DeveloperSettings settings = DeveloperSettings.GetSettings();
      key = GetCharacterKey(settings.ProtagonistProfile);
      MapCache.Add(key, GameManager.Player.Animator);

      if (GameManager.Companion != null) {
        key = GetCharacterKey(settings.CompanionProfile);
        if (!MapCache.ContainsKey(key)) {
          MapCache.Add(key, GameManager.Companion.Animator);
        }
      }
    }

    private string GetCharacterKey(CharacterProfile character) => character.Category.ToString() + "/" + character.CharacterName;

    public override bool IsNodeComplete() {
      if (Mapping == null) {
        return false;
      }

      foreach (var pair in Mapping) {
        if (pair == null || pair.Actor == null || pair.Profile == null) {
          return false;
        }
      }

      return true;
    }
  }
}
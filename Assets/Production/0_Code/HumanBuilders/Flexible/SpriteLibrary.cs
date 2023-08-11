using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [Serializable]
  [CreateAssetMenu(fileName = "New Sprite Library", menuName = "Journey/Sprite Library")]
  public class SpriteLibrary : ScriptableObject {
    public string Category;
    
    [TableList(AlwaysExpanded = true)]
    public List<Sprite> Sprites;

    public Sprite this [int index] {
      get {
        return Sprites[index];
      }
    }

    public int Count {
      get {return Sprites.Count;}
    }
  }
}
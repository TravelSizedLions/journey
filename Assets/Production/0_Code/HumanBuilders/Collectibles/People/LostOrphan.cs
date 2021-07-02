using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {
  public class LostOrphan : LostPerson {
    private const string ORPHAN="orphan";

    protected override void Awake() {
      // Since there will be a ton of orphans and are a special collectible,
      // separating them out from other people will help organize the save file.
      key = ORPHAN+"_"+Name;
      base.Awake();
    }
  }
}
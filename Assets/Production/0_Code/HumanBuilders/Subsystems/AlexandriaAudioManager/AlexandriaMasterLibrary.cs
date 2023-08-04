using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  [CreateAssetMenu(fileName = "alexandria", menuName = "Alexandria/Create Master Library")]
  public class AlexandriaMasterLibrary : ScriptableObject {
    public static AlexandriaMasterLibrary Get() {
      return Resources.Load<AlexandriaMasterLibrary>("alexandria");
    }

    [TableList(AlwaysExpanded=false)]
    public List<SoundLibrary> Libraries;
  }
}
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {

  [CreateAssetMenu(fileName="New Concept", menuName="Dialog/Concept", order=1)]
  public class Concept : ScriptableObject {
    public string ConceptName;

    [Multiline(10)]
    public string Description;
  }
}
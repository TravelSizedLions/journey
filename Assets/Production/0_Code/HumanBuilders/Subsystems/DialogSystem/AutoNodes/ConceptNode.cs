using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace HumanBuilders {
  [NodeWidth(300)]
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [CreateNodeMenu("Dialog/Concept Reference")]
  public class ConceptNode : AutoValueNode {

    public override object Value { get => Concept.ConceptName; }

    [Space(8)]
    [ValueDropdown("GetConcepts")]
    [SerializeField]
    [Tooltip("The concept or item to reference.")]
    private Concept Concept;

    public override bool IsNodeComplete() {
      return base.IsNodeComplete() && 
             Concept != null;
    }

#if UNITY_EDITOR    
    //---------------------------------------------------------------------
    // Odin Inspector
    //---------------------------------------------------------------------
    public IEnumerable GetConcepts() => EditorUtils.FindAssetsByType<Concept>();
#endif
  }
}
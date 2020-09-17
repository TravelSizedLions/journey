using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Storm.Characters.Bosses {
  [Serializable]
  public class BossAttack {
    [LabelText("%")]
    [TableColumnWidth(80, Resizable = false)]
    [Range(0, 100)]
    public float Frequency;

    [SuffixLabel("Stylized Name", true)]
    [LabelText(" ")]
    [LabelWidth(1)]
    [HorizontalGroup("Attack Info")]
    public string Name;

    [SuffixLabel("Trigger Parameter", true)]
    [LabelText(" ")]
    [LabelWidth(10)]
    [HorizontalGroup("Attack Info")]
    public string Trigger;
  }
}
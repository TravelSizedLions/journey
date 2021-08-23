using System;
using Sirenix.OdinInspector;

namespace HumanBuilders {
  [Serializable]
  public class SlideShowTextEntry {
    [LabelText("Text")]
    [LabelWidth(50)]
    [MultiLineProperty(3)]
    public string Text;
  }
}
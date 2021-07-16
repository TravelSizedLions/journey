
using System;

namespace HumanBuilders {
  public enum VariableType { Boolean, Float, Integer, String, GUID };

  public interface IVariable : IObservable<IVariable> {
    string Folder { get; set; }

    string Key { get; set; }

    VariableType Type { get; set; }

    dynamic Value { get; set; }

    bool BoolValue { get; set; }

    float FloatValue { get; set; }

    int IntegerValue { get; set; }

    string StringValue { get; set; }

    GuidReference GUIDValue { get; set; }
  }
}
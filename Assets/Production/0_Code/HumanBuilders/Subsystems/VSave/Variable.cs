using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders {

  [CreateAssetMenu(fileName = "New VSave Variable", menuName = "VSave/Variable")]
  public class Variable : ScriptableObject, IVariable {

    [BoxGroup("Locator")]
    [ValueDropdown("GetStaticFolders")]
    [PropertyOrder(0)]
    [ShowInInspector]
    public virtual string Folder { get; set; }

    [BoxGroup("Locator")]
    [PropertyOrder(0)]
    [ShowInInspector]
    public virtual string Key { get; set; }

    [BoxGroup("Value")]
    [PropertyOrder(998)]
    [ShowInInspector]
    public virtual VariableType Type { get; set; } = VariableType.Boolean;

    public dynamic Value {
      get {
        switch (Type) {
          case VariableType.Boolean:
            return BoolValue;
          case VariableType.Integer:
            return IntegerValue;
          case VariableType.Float:
            return FloatValue;
          case VariableType.String:
            return StringValue;
          case VariableType.GUID:
            return GUIDValue;
        }

        return null;
      }

      set {
        switch (Type) {
          case VariableType.Boolean:
            BoolValue = value;
            return;
          case VariableType.Integer:
            IntegerValue = value;
            return;
          case VariableType.Float:
            FloatValue = value;
            return;
          case VariableType.String:
            StringValue = value;
            return;
          case VariableType.GUID:
            GUIDValue = value;
            return;
        }
      }
    }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.Boolean)]
    [ShowInInspector]
    public virtual bool BoolValue {
      get => VSave.Get<bool>(Folder, Key);
      set => VSave.Set(Folder, Key, value);
    }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.Float)]
    [ShowInInspector]
    public virtual float FloatValue {
      get => VSave.Get<float>(Folder, Key);
      set => VSave.Set(Folder, Key, value);
    }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.Integer)]
    [ShowInInspector]
    public virtual int IntegerValue {
      get => VSave.Get<int>(Folder, Key);
      set => VSave.Set(Folder, Key, value);
    }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.String)]
    [ShowInInspector]
    public virtual string StringValue {
      get => VSave.Get<string>(Folder, Key);
      set => VSave.Set(Folder, Key, value);
    }

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.GUID)]
    [ShowInInspector]
    public virtual GuidReference GUIDValue {
      get {
        if (VSave.Get(Folder, Key, out byte[] bytes) && bytes != null) {
          return new GuidReference(bytes);
        }

        return null;
      }
      set => VSave.Set(Folder, Key, value.ToByteArray());
    }

    [PropertyOrder(1000)]
    [AutoTable(typeof(IObserver<IVariable>))]
    [ShowInInspector]
    public AutoTable<IObserver<IVariable>> Observers;

    public virtual IDisposable Subscribe(IObserver<IVariable> observer) {
      Observers.Add(observer);
      return new Unsubscriber(Observers, observer);
    }

    private IEnumerable<string> GetStaticFolders() {
      List<string> folders = new List<string>();

      // Gets the list of constant fields that represent all the game's data folders.
      FieldInfo[] fields = typeof(StaticFolders).GetFields(BindingFlags.Public | BindingFlags.Static);
      foreach (FieldInfo field in fields) {
        if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string)) {
          folders.Add((string) field.GetRawConstantValue());
        }
      }

      return folders;
    }

    private class Unsubscriber : IDisposable {
      private AutoTable<IObserver<IVariable>> _observers;
      private IObserver<IVariable> _observer;

      public Unsubscriber(AutoTable<IObserver<IVariable>> observers, IObserver<IVariable> observer) {
        this._observers = observers;
        this._observer = observer;
      }

      public void Dispose() {
        if (_observer != null && _observers.Contains(_observer))
          _observers.Remove(_observer);
      }
    }

    //-------------------------------------------------------------------------
    // Odin Stuff
    //-------------------------------------------------------------------------
    #if UNITY_EDITOR
    [BoxGroup("Locator")]
    [PropertyOrder(1)]
    [Button("Name to Key")]
    public virtual void NameToKey() {
      Key = name;
    }
    #endif
  }
}
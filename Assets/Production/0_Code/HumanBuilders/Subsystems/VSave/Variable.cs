using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using HumanBuilders.Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HumanBuilders {

  [CreateAssetMenu(fileName = "New VSave Variable", menuName = "Variable/V-Variable")]
  [Serializable]
  public class Variable : ScriptableObject, IObservable<Variable> {
    //-------------------------------------------------------------------------
    // Locator Properties
    //-------------------------------------------------------------------------
    [BoxGroup("Locator")]
    [ValueDropdown("GetStaticFolders")]
    [PropertyOrder(0)]
    [OdinSerialize]
    public string Folder;

    [BoxGroup("Locator")]
    [PropertyOrder(0)]
    [HideIf("UseCustomKey")]
    [ValueDropdown("GeyKeyPresets")]
    [LabelText("Key")]
    [OdinSerialize]
    public string PresetKey;

    [BoxGroup("Locator")]
    [PropertyOrder(0)]
    [ShowIf("UseCustomKey")]
    [LabelText("Key")]
    [OdinSerialize]
    public string CustomKey;

    [BoxGroup("Locator")]
    [PropertyOrder(0)]
    [OdinSerialize]
    public bool UseCustomKey;

    public virtual string Key { 
      get => UseCustomKey ? CustomKey : PresetKey; 
      set { if (UseCustomKey) CustomKey = value; else PresetKey = value; }
    }

    //-------------------------------------------------------------------------
    // Value Properties
    //-------------------------------------------------------------------------
    [BoxGroup("Value")]
    [PropertyOrder(997)]
    public VariableType Type = VariableType.Boolean;

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
            break;
          case VariableType.Integer:
            IntegerValue = (int)value;
            break;
          case VariableType.Float:
            FloatValue = value;
            break;
          case VariableType.String:
            StringValue = value;
            break;
          case VariableType.GUID:
            GUIDValue = value;
            break;
        }
      }
    }

    public dynamic DefaultValue {
      get {
        switch (Type) {
          case VariableType.Boolean:
            return DefaultBoolValue;
          case VariableType.Integer:
            return DefaultIntValue;
          case VariableType.Float:
            return DefaultFloatValue;
          case VariableType.String:
            return DefaultStringValue;
          case VariableType.GUID:
            return DefaultGUIDValue;
        }

        return null;
      }
    }

    // --- Bool ---
    [BoxGroup("Value")]
    [PropertyOrder(998)]
    [ShowIf("Type", VariableType.Boolean)]
    [LabelText("Default Value")]
    public bool DefaultBoolValue;

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.Boolean)]
    [LabelText("Current Value")]
    [ShowInInspector]
    public virtual bool BoolValue {
      get => VSave.Get<bool>(Folder, Key);
      set {
        VSave.Set(Folder, Key, value);
        NotifyObservers();
      }
    }

    // --- Float ---
    [BoxGroup("Value")]
    [PropertyOrder(998)]
    [ShowIf("Type", VariableType.Float)]
    [LabelText("Default Value")]
    public float DefaultFloatValue;

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.Float)]
    [LabelText("Current Value")]
    [ShowInInspector]
    public virtual float FloatValue {
      get => VSave.Get<float>(Folder, Key);
      set {
        VSave.Set(Folder, Key, value);
        NotifyObservers();
      }
    }

    // --- Integer ---
    [BoxGroup("Value")]
    [PropertyOrder(998)]
    [ShowIf("Type", VariableType.Integer)]
    [LabelText("Default Value")]
    public int DefaultIntValue;

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.Integer)]
    [LabelText("Current Value")]
    [ShowInInspector]
    public virtual int IntegerValue {
      get => VSave.Get<int>(Folder, Key);
      set {
        VSave.Set(Folder, Key, value);
        NotifyObservers();
      }
    }

    // --- String ---
    [BoxGroup("Value")]
    [PropertyOrder(998)]
    [ShowIf("Type", VariableType.String)]
    [LabelText("Default Value")]
    public string DefaultStringValue;

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.String)]
    [LabelText("Current Value")]
    [ShowInInspector]
    public virtual string StringValue {
      get => VSave.Get<string>(Folder, Key);
      set {
        VSave.Set(Folder, Key, value);
        NotifyObservers();
      }
    }

    // --- GUID ---
    [BoxGroup("Value")]
    [PropertyOrder(998)]
    [ShowIf("Type", VariableType.GUID)]
    [LabelText("Default Value")]
    public GuidReference DefaultGUIDValue;

    [BoxGroup("Value")]
    [PropertyOrder(999)]
    [ShowIf("Type", VariableType.GUID)]
    [LabelText("Current Value")]
    public virtual GuidReference GUIDValue {
      get {
        if (VSave.Get(Folder, Key, out byte[] bytes) && bytes != null) {
          guid = new GuidReference(bytes);
        } else if (guid == null) {
          guid = new GuidReference();
        }

        return guid;
      }
      set {
        VSave.Set(Folder, Key, value.ToByteArray());
        guid = value;
        NotifyObservers();
      }
    }

    protected GuidReference guid;

    [FoldoutGroup("Context")]
    [HideLabel]
    [TextArea(3, 10)]
    [PropertyOrder(1000)]
    public string Context;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    #if UNITY_EDITOR 
    private void OnEnable() {
      EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange change) {
      switch(change) {
        case PlayModeStateChange.EnteredPlayMode:
          Debug.Log("Initializing");
          Initialize();
          break;
        case PlayModeStateChange.ExitingPlayMode:
          Debug.Log("Exiting");
          EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
          break;
      }
    }

    #else
    private void OnEnable() {
      Initialize();
    }
    #endif

    public void Initialize() {
      if (string.IsNullOrEmpty(Folder) || string.IsNullOrEmpty(Key)) {
        if (Application.isPlaying) {
          Debug.LogWarning("Variable \"" + name +  "\" is missing either a Folder or Key value.");
        }
        return;
      }

      if (Application.isPlaying && !IsSet() && DefaultValue != null) {
        Value = DefaultValue;
      }
    }

    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    public bool IsSet() {
      switch (Type) {
        case VariableType.Boolean: return VSave.IsSet<bool>(Folder, Key);
        case VariableType.Integer: return VSave.IsSet<int>(Folder, Key);
        case VariableType.Float: return VSave.IsSet<float>(Folder, Key);
        case VariableType.String: return VSave.IsSet<string>(Folder, Key);
        case VariableType.GUID: return VSave.IsSet<byte[]>(Folder, Key);
      }

      return false;
    }

    //-------------------------------------------------------------------------
    // Observable Stuff
    //-------------------------------------------------------------------------
    [FoldoutGroup("Observers")]
    [HideLabel]
    [PropertyOrder(1001)]
    [AutoTable(typeof(IObserver<IVariable>))]
    [ShowInInspector]
    public List<IObserver<Variable>> Observers;

    public virtual IDisposable Subscribe(IObserver<Variable> observer) {
      Observers = Observers ?? new List<IObserver<Variable>>();
      Observers.Add(observer);
      return null;
    }

    public virtual void Unsubscribe(IObserver<Variable> observer) {
      if (Observers != null && Observers.Contains(observer)) {
        Observers.Remove(observer);
      }
    }

    public virtual void NotifyObservers() {
      if (Observers != null) {
        var observers = new IObserver<Variable>[Observers.Count];
        Observers.CopyTo(observers);
        foreach (var obs in observers) {
          obs.OnNext(this);
        }
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

    public virtual List<string> GeyKeyPresets() {
      List<string> keys = new List<string>();
      foreach (var prop in typeof(Keys).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)) {
        if (prop.IsLiteral && !prop.IsInitOnly) {
          keys.Add((string)prop.GetRawConstantValue());
        }
      }
      return keys;
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
    #endif
  }
}
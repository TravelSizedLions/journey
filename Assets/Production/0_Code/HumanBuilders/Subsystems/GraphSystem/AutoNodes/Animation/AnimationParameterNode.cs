using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;
#endif

namespace HumanBuilders {

  /// <summary>
  /// A node for switching the animation on a controller in the scene.
  /// </summary>
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Animation/Set Animation Param")]
  public class AnimationParameterNode : AutoNode {
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The Animator controller to change. If left blank, the default is the
    /// player character's animator.
    /// </summary>
    [Tooltip("The Animator controller to change.")]
    [SerializeField]
    [OnValueChanged(nameof(Params))]
    public Animator Animator;

    /// <summary>
    /// The name of the parameter to set.
    /// </summary>
    [Tooltip("The name of the parameter to set.")]
    [SerializeField]
    [ValueDropdown(nameof(Params))]
    [OnValueChanged(nameof(SetParamType))]
    public string Parameter;

    [SerializeField]
    [HideInInspector]
    private AnimatorControllerParameterType parameterType = AnimatorControllerParameterType.Trigger;

    /// <summary>
    /// The boolean to set in the Animator.
    /// </summary>
    [Tooltip("The boolean to set.")]
    [SerializeField]
    [LabelText("Value")]
    [ShowIf(nameof(parameterType), AnimatorControllerParameterType.Bool)]
    public bool BoolValue;

    [Tooltip("The float to set.")]
    [SerializeField]
    [LabelText("Value")]
    [ShowIf(nameof(parameterType), AnimatorControllerParameterType.Float)]
    public float FloatValue;

    [Tooltip("The float to set.")]
    [SerializeField]
    [LabelText("Value")]
    [ShowIf(nameof(parameterType), AnimatorControllerParameterType.Int)]
    public int IntValue;

    /// <summary>
    /// The output connection for this node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;


    //---------------------------------------------------
    // Unity API
    //---------------------------------------------------
    private void Awake() {

    }

    //---------------------------------------------------
    // Auto Node API
    //---------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (Animator == null) {
        if (player == null) {
          player = GameManager.Player;
        }

        Animator = player.GetComponent<Animator>();
      }
  
      switch(parameterType) {
        case AnimatorControllerParameterType.Bool:
          Animator.SetBool(Parameter, BoolValue);
          break;
        case AnimatorControllerParameterType.Float:
          Animator.SetFloat(Parameter, FloatValue);
          break;
        case AnimatorControllerParameterType.Int:
          Animator.SetInteger(Parameter, IntValue);
          break;
        case AnimatorControllerParameterType.Trigger:
          Animator.SetTrigger(Parameter);
          break;
        default:
          Debug.LogWarning("No Parameter Selected!");
          break;
      }
    }

    //-------------------------------------------------------------------------
    // Odin Inspector Stuff
    //-------------------------------------------------------------------------
    private IEnumerable Params() {
      List<string> parms = new List<string>();

      #if UNITY_EDITOR
      if (Animator != null) {
        if (Animator.parameters.Length > 0) {
          foreach (var param in Animator.parameters) {
            parms.Add(param.name);
          }
        } else {
          AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(
            AssetDatabase.GetAssetPath(Animator.runtimeAnimatorController)
          );

          foreach (var param in controller.parameters) {
            parms.Add(param.name);
          }
        }
        
      }

      if (Parameter == null) {
        Parameter = "-- select a parameter --";
      }
      #endif

      return parms;
    }


    private void SetParamType() {
      #if UNITY_EDITOR
      if (Animator != null) {
        foreach (var param in Animator.parameters) {
          if (param.name == Parameter) {
            parameterType = param.type;
          }
        }
      }
      #endif
    }
  }
}
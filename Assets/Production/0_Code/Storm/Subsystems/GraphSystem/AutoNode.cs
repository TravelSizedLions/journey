using System.Collections;
using System.Collections.Generic;
using Storm.Characters.Player;
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Graph {

  /// <summary>
  /// The base class for Graph Nodes. Follows the "Template Method" pattern.
  /// </summary>
  /// <remarks>
  /// Template Method Pattern: https://sourcemaking.com/design_patterns/template_method
  /// </remarks>
  public abstract class AutoNode : Node, IAutoNode {

    #region Fields
    //---------------------------------------------------------------------
    // Fields
    //---------------------------------------------------------------------
    
    /// <summary>
    /// The list of conditions to check. The conditions will be checked each
    /// frame in the order listed. If any is met, the transition that
    /// corresponds to the condition will be used.
    /// </summary>
    /// <remarks>
    /// To keep things looking clean in the inspector, this variable is kept
    /// private for the following reasons:
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///     Some nodes already have predefined transitions.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     Not all nodes need the flexibility of being able to dynamically
    /// define how they transition to different nodes.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     Not all nodes SHOULD ALLOW for different ways to transition to
    ///     different nodes.
    ///     </description>
    ///   </item>
    /// </list>
    /// 
    /// To create a node that implements dynamic conditions:
    /// <list type="number">
    ///   <item>
    ///     <description>
    ///     Sub-class from <see cref="AutoNode" />.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     Add a public facing list of conditions to your node so you can add
    ///     conditions in the inspector.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     In the Awake() or Start() method for your node, make the call to 
    ///     <see cref="IAutoNode.RegisterConditions" />.
    ///     </description>
    ///   </item>
    /// </list>
    /// </remarks>
    private List<Condition> registeredConditions = null;

    /// <summary>
    /// A reference to the Player.
    /// </summary>
    protected static PlayerCharacter player;

    /// <summary>
    /// A reference to the graph traversal engine that last handled this node. 
    /// </summary>
    private GraphEngine engine;
    #endregion

    #region XNode API
    //---------------------------------------------------------------------
    // XNode API
    //---------------------------------------------------------------------
    
    public override object GetValue(NodePort port) {
      return null;
    }

    #endregion
      
    #region Dialog Node API
    //---------------------------------------------------------------------
    // Dialog Node API
    //---------------------------------------------------------------------

    /// <summary>
    /// Handle this node.
    /// </summary>
    /// <param name="graphEngine">
    /// The graph engine that called this into this node.
    /// </param>
    /// <remarks>
    /// This is a template method (see
    /// https://sourcemaking.com/design_patterns/template_method). Sub-class
    /// from this class and override Handle() and PostHandle() to create your
    /// own custom behavior.
    /// </remarks>
    public void HandleNode(GraphEngine graphEngine) {
      if (player == null) {
        player = GameManager.Player;
      }

      if (graphEngine.StartHandlingNode()) {

        // Hook method. Implement this in a sub-class.
        engine = graphEngine;
        Handle(graphEngine);

        /** 
         * Some nodes spin off coroutines in their Handle() method. 
         * 
         * When this is the case, it's possible for the node to lock "finishing" the node
         * until the coroutine is done. In this case, we spin up a second
         * coroutine to wait until the node truly is finished.
        */
        if (graphEngine.FinishHandlingNode()) {
          // Hook method. Implement this in a sub-class.
          PostHandle(graphEngine);
        } else {
          graphEngine.StartThread(_WaitUntilFinished(graphEngine));
        }
      }
    }

    /// <summary>
    /// How to handle this node.
    /// </summary>
    /// <param name="graphEngine">The graph traversal engine that called this node</param>
    /// <remarks>
    /// This is a hook method. Override this in a sub-class of <see cref="AutoNode"/> in
    /// order to write the actual behavior of the node. 
    /// </remarks>
    public virtual void Handle(GraphEngine graphEngine) {

    }

    /// <summary>
    /// What to do after handling this node.
    /// </summary>
    /// <param name="graphEngine">
    /// The graph engine that called this into this node.
    /// </param>
    /// <remarks>
    /// Usually, this will either be "go to the next node in the graph" or 
    /// "do nothing (and wait for the next player input)." The default behavior
    /// handles the first case.
    /// <para/>
    /// This is a hook method. Override this in a sub-class of <see cref="AutoNode"/> in
    /// order to write the actual behavior of the node. 
    /// </remarks>
    public virtual void PostHandle(GraphEngine graphEngine) {
      IAutoNode node = GetNextNode();
      graphEngine.SetCurrentNode(node);
      graphEngine.Continue();
    }

    /// <summary>
    /// Get the next node in the dialog graph.
    /// </summary>
    /// <returns>The next node in the dialog graph.</returns>
    public virtual IAutoNode GetNextNode() {
      return (IAutoNode)GetOutputPort("Output").Connection.node;
    }

    /// <summary>
    /// Waits to call the PostHandle hook until after the node has actually
    /// finished handling itself.
    /// </summary>
    /// <returns></returns>
    private IEnumerator _WaitUntilFinished(GraphEngine graphEngine) {
      while(!graphEngine.FinishHandlingNode()) {
        yield return null;
      }

      PostHandle(graphEngine);
    }

    /// <summary>
    /// Add a list of conditional transitions to this node. These conditions
    /// will be checked each frame.
    /// </summary>
    /// <param name="conditions">The list of conditions to register.</param>
    /// <param name="outputPort">The name of the output port these conditions
    /// map to.</param>
    public void RegisterConditions(List<Condition> conditions, string outputPort) {
      if (registeredConditions == null) {
        registeredConditions = new List<Condition>();
      }

      for (int i = 0; i < conditions.Count; i++) {
        conditions[i].OutputPort = outputPort+" "+i;
      }

      registeredConditions.AddRange(conditions);
    }

    /// <summary>
    /// Check any transition conditions registered on this node. 
    /// </summary>
    /// <returns>True if any condition was met. False otherwise.</returns>
    public bool CheckConditions() {
      if (registeredConditions != null) {
        // If there are any registered conditions, check them to see if the node
        // should transition.
        foreach (Condition c in registeredConditions) {
          if (c.ConditionMet()) {
            Debug.Log("Condition Met!!");
            c.Transition(engine, this);
            return true;
          }
        }
      }

      return false;
    }
    #endregion
  }
}

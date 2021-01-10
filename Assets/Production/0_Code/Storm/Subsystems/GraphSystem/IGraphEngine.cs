


namespace HumanBuilders {
  /// <summary>
  /// This interface defines the API for a graph traversal engine. This engine
  /// will take an <see cref="AutoGraph" /> and traverse through it from start
  /// to finish. Useful for dialog and in-game cutscenes.
  /// </summary>
  public interface IGraphEngine {

    #region Dependency Injection
    //-------------------------------------------------------------------------
    // Dependency Injection
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Inject a graph (used for automated testing -- for normal dev, use <see cref="GraphEngine.SetCurrentGraph" /> instead).
    /// </summary>
    void Inject(IAutoGraph graph);


    /// <summary>
    /// Inject a node to be the current node (used for automated testing -- for
    /// normal dev, use <see cref="GraphEngine.SetCurrentNode" /> instead).
    /// </summary>
    void Inject(IAutoNode node);
    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Begin traversing a new graph.
    /// </summary>
    /// <seealso cref="GraphEngine.StartGraph" />
    void StartGraph(IAutoGraph graph);

    /// <summary>
    /// Continue traversing the current graph.
    /// </summary>
    /// <seealso cref="GraphEngine.Continue" />
    void Continue();

    /// <summary>
    /// Set the current node for the graph. Don't use this while in the middle
    /// of traversing another graph.
    /// </summary>
    /// <seealso cref="GraphEngine.SetCurrentNode" />
    void SetCurrentNode(IAutoNode node);
    
    /// <summary>
    /// Get the current node in the graph.
    /// </summary>
    /// <seealso cref="GraphEngine.GetCurrentNode" />
    IAutoNode GetCurrentNode();

    /// <summary>
    /// Set the current graph to be traversed.
    /// Don't use this while in the middle of travering another graph.
    /// </summary>
    /// <seealso cref="GraphEngine.SetCurrentGraph" />
    void SetCurrentGraph(IAutoGraph graph);

    /// <summary>
    /// Get the graph that's currently being traversed.
    /// </summary>
    /// <seealso cref="GraphEngine.GetCurrentGraph" />
    IAutoGraph GetCurrentGraph();

    /// <summary>
    /// Whether or not the graph is finished being traversed.
    /// </summary>
    /// <seealso cref="GraphEngine.Finished" />
    bool IsGraphFinished();

    /// <summary>
    /// Locks handling a dialog. This will prevent more nodes from being fired
    /// in a conversation until the lock has been released.
    /// </summary>
    /// <returns>True if the lock was obtained, false otherwise.</returns>
    /// <seealso cref="GraphEngine.LockNode" />
    bool LockNode();

    /// <summary>
    /// Unlocks handling a dialog. If there was previously a lock on firing more
    /// nodes in the conversation, this will release it.
    /// </summary>
    /// <remarks>
    /// Don't use this without first trying to obtain the lock for yourself.
    /// </remarks>
    /// <returns>
    /// Whether or not the current node was locked.
    /// </returns>
    /// <seealso cref="GraphEngine.UnlockNode" />
    bool UnlockNode();

    /// <summary>
    /// Try to start handling a node in the conversation.
    /// </summary>
    /// <returns>
    /// True if previous node in the conversation graph is finished being handled. False otherwise.
    /// </returns>
    /// <seealso cref="GraphEngine.StartHandlingNode" />
    bool StartHandlingNode();

    /// <summary>
    /// Try to finish handling a node in the conversation.
    /// </summary>
    /// <returns>
    /// True if the current node finished handling successfully. False if the current node still needs time to finish.
    /// </returns>
    /// <seealso cref="GraphEngine.FinishHandlingNode" />
    bool FinishHandlingNode();
    #endregion
  }
}
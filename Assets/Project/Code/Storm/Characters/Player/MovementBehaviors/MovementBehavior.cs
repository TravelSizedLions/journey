
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Characters.Player {

  /// <summary>
  /// 
  /// </summary>
  public abstract class MovementBehavior : PlayerBehavior {

    /// <summary>
    /// The symbol stack for 
    /// </summary>
    /// <typeparam name="MovementSymbol">An enumerator representing the set of pushdown symbols for this pushdown automata.</typeparam>
    protected static Stack<MovementSymbol> stack = new Stack<MovementSymbol>();

    protected static int StackSize {
      get { return stack.Count; }
    }

    public virtual void HandlePhysics() {
      
    }

    /// <summary>
    /// Change state. The old state behavior will be detached from the player after this call.
    /// </summary>
    protected void ChangeState<State>() where State : MovementBehavior {
      Debug.Log(typeof(State));
      player.OnStateChange(this, player.gameObject.AddComponent<State>());
    }


    /// <summary>
    /// Check if the top symbol on the stack matches the target. If it does, consume it.
    /// </summary>
    /// <param name="targetSymbol">The symbol to consume.</param>
    /// <returns>True if the symbol was consumed from the stack.</returns>
    protected bool TryConsume(MovementSymbol targetSymbol) {
      if (StackSize > 0) {
        MovementSymbol symbol = Pop();
        if (symbol == targetSymbol) {
          return true;
        } 

        Push(symbol);
        return false;
      }

      return false;
    }


    /// <summary>
    /// Push a symbol onto the stack.
    /// </summary>
    /// <param name="symbol">The symbol to push.</param>
    protected void Push(MovementSymbol symbol) {
      stack.Push(symbol);
    }

    /// <summary>
    /// Pop a symbol off of the stack.
    /// </summary>
    /// <returns>The topmost symbol on the stack.</returns>
    protected MovementSymbol Pop() {
      return stack.Pop();
    }
  }
}
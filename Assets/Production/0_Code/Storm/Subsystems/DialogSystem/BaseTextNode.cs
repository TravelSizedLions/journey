using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using XNode;

namespace Storm.Subsystems.Dialog {
  /// <summary>
  /// The base class that carries shared behavior between dialog nodes that
  /// display some kind of text.
  /// </summary>
  public class BaseTextNode : DialogNode {

    protected void TypeSentence(string text) {
      if (manager == null) {
        manager = DialogManager.Instance;
      }


      if (!manager.HandlingConversation) {
        manager.HandlingConversation = true;

        if (manager.StillWriting && !manager.IsFinishedTyping(text)) {
          manager.SkipTyping(text);
          TryListDecisions();
        } else {
          manager.StopTyping();
          manager.StartTyping(_TypeSentence(text));
        }

        manager.HandlingConversation = false;
      }
    }

    public void TryListDecisions() {
      var node = GetNextNode();

      if (node is DecisionNode decisions) {
        manager.ListDecisions(decisions);
      }

      manager.SetCurrentNode(node);
    }

    /// <summary>
    /// A coroutine to type a sentence onto the screen character by character.
    /// </summary>
    /// <param name="sentence">The sentence to type</param>
    IEnumerator _TypeSentence(string sentence) {

      manager.HandlingConversation = true;
      manager.StillWriting = true;
      manager.ClearText();

      if (sentence != null) {
        foreach (char c in sentence.ToCharArray()) {
          manager.Type(c);
          yield return null;
        }
      }

      TryListDecisions();

      manager.StillWriting = false;
      manager.HandlingConversation = false;
    }
  }
}
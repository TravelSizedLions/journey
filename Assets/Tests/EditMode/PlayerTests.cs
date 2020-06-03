using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;

namespace Tests.EditMode {
  public class PlayerTests {
    // A Test behaves as an ordinary method
    [Test]
    public void NewTestScriptSimplePasses() {
      // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator Moves_Along_X_Axis_With_Horizontal_Input() {

      GameObject playerObject = new GameObject();
      playerObject.AddComponent<Rigidbody2D>();
      playerObject.AddComponent<SpriteRenderer>();
      playerObject.AddComponent<Animator>();
      playerObject.AddComponent<MovementSettings>();
      
      PlayerCharacter player =  playerObject.AddComponent<PlayerCharacter>();

      yield return null;
    }
  }
}
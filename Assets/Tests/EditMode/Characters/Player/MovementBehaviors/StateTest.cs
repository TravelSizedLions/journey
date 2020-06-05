using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Services;

namespace Tests {

  public class StateTest<State> where State : PlayerState {

    protected GameObject go;

    protected State state;

    protected IPlayer player;

    protected UnityPhysics physics;

    protected MovementSettings settings;


    protected void SetupTest() {
      go = new GameObject();
      state = go.AddComponent<State>();
      player = Substitute.For<IPlayer>();

      physics = go.AddComponent<UnityPhysics>();
      physics.Awake();

      settings = go.AddComponent<MovementSettings>();

      state.Inject(player, physics);
    }

    // A Test behaves as an ordinary method
    [Test]
    public void StateTestSimplePasses() {
      // Use the Assert class to test conditions
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator StateTestWithEnumeratorPasses() {
      // Use the Assert class to test conditions.
      // Use yield to skip a frame.
      yield return null;
    }
  }

}
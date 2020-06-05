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

    /// <summary>
    /// Asserts that the player received a request to change to the provided state
    /// </summary>
    /// <typeparam name="NextState">The expected state transition</typeparam>
    protected void AssertStateChange<NextState>() where NextState : PlayerState {
      player.Received().OnStateChange(Arg.Any<State>(), Arg.Any<NextState>());
    }

    /// <summary>
    /// Asserts that the player did not receive a request to change to the provided state
    /// </summary>
    /// <typeparam name="NextState">The expected state transition</typeparam>
    protected void AssertNoStateChange<NextState>() where NextState : PlayerState {
      player.DidNotReceive().OnStateChange(Arg.Any<State>(), Arg.Any<NextState>());
    }
  }

}
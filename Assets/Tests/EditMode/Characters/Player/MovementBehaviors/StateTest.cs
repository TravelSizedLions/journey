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
  }

}
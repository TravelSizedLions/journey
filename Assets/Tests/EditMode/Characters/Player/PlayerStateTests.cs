using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Services;
using Storm.Subsystems.FSM;

using Tests.FSM;

namespace Tests.Characters.Player {
    public class PlayerStateTest<S> : StateTest<S> where S : PlayerState {

    protected IPlayer player;

    protected UnityPhysics physics;

    protected MovementSettings settings;

    protected override void SetupTest() {
      base.SetupTest();

      player = Substitute.For<IPlayer>();

      physics = go.AddComponent<UnityPhysics>();
      physics.Awake();

      settings = go.AddComponent<MovementSettings>();

      state.Inject(player, physics);
    }

  }
}
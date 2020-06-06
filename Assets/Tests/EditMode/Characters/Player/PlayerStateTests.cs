using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Components;
using Storm.Subsystems.FSM;

using Tests.FSM;

namespace Tests.Characters.Player {
    public class PlayerStateTest<S> : StateTest<S> where S : PlayerState {

    protected IPlayer player;

    protected PhysicsComponent physics;

    protected MovementSettings settings;

    protected override void SetupTest() {
      base.SetupTest();

      player = Substitute.For<IPlayer>();

      physics = go.AddComponent<PhysicsComponent>();
      physics.Awake();

      settings = go.AddComponent<MovementSettings>();

      state.Inject(player, physics);
    }

  }
}
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
using Storm.Flexible;

namespace Tests.Characters.Player {
    public class PlayerStateTest<S> : StateTest<S> where S : PlayerState {

    protected IPlayer player;

    protected PhysicsComponent physics;

    protected MovementSettings movementSettings;

    protected CarrySettings carrySettings;

    protected override void SetupTest() {
      base.SetupTest();

      player = Substitute.For<IPlayer>();

      physics = go.AddComponent<PhysicsComponent>();
      physics.Awake();

      movementSettings = go.AddComponent<MovementSettings>();
      carrySettings = go.AddComponent<CarrySettings>();

      state.Inject(player, physics);
    }


    protected Carriable BuildCarriable() {
      GameObject g = new GameObject();

      Carriable c = g.AddComponent<Carriable>();

      Rigidbody2D r = g.AddComponent<Rigidbody2D>();
      BoxCollider2D b = g.AddComponent<BoxCollider2D>();
      PhysicsComponent p = g.AddComponent<PhysicsComponent>();
      
      c.Inject(b);
      c.Physics = p;
      c.Physics.Inject(r);

      return c;
    }

  }
}
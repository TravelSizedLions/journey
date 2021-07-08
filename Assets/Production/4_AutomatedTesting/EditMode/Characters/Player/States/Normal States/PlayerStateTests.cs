using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;
using HumanBuilders;





using HumanBuilders.Tests;



namespace HumanBuilders.Tests {
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

      player.MovementSettings = settings;

      state.Inject(player, physics, settings);
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
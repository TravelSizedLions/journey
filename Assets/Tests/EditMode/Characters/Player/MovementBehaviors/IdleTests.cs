using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Services;

namespace Tests {
  public class IdleTests {

    [Test]
    public void Idle_Can_Jump() {

      // Setup;
      Idle state = new GameObject().AddComponent<Idle>();

      // Mock
      IPlayer player = Substitute.For<IPlayer>();
      IPhysics physics = Substitute.For<IPhysics>();
      state.Inject(player, physics);

      // Inputs
      player.PressedJump().Returns(true);

      // Perform
      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Idle>(), Arg.Any<Jump1Start>());
    }


    [Test]
    public void Idle_Can_Run() {
      Idle state = new GameObject().AddComponent<Idle>();

      IPlayer player = Substitute.For<IPlayer>();
      IPhysics physics = Substitute.For<IPhysics>();
      state.Inject(player, physics);

      player.TryingToMove().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Idle>(), Arg.Any<Running>());
    }


    [Test]
    public void Idle_Can_Crouch() {
      Idle state = new GameObject().AddComponent<Idle>();

      IPlayer player = Substitute.For<IPlayer>();
      IPhysics physics = Substitute.For<IPhysics>();
      state.Inject(player, physics);

      player.HoldingDown().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Idle>(), Arg.Any<CrouchStart>());
    }


    [Test]
    public void Idle_Can_WallRun() {
      Idle state = new GameObject().AddComponent<Idle>();

      IPlayer player = Substitute.For<IPlayer>();
      IPhysics physics = Substitute.For<IPhysics>();
      state.Inject(player, physics);

      player.PressedJump().Returns(true);
      player.IsTouchingLeftWall().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Idle>(), Arg.Any<WallRun>());
    }

    [UnityTest]
    public IEnumerator Idle_Velocity_Zero() {
      GameObject go = new GameObject();
      Idle state = go.AddComponent<Idle>();

      IPlayer player = Substitute.For<IPlayer>();
      UnityPhysics physics = go.AddComponent<UnityPhysics>();

      physics.Awake();

      state.Inject(player, physics);

      state.OnStateEnter();

      yield return null;

      Assert.AreEqual(physics.Velocity, new Vector2(0, 0));
    }
  }
}
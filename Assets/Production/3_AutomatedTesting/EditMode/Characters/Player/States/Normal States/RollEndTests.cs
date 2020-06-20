using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Components;

namespace Tests.Characters.Player {
  public class RollEndTests : PlayerStateTest<RollEnd> {

    [Test]
    public void RollEnd_Can_Jump() {
      SetupTest();

      player.HoldingJump().Returns(true);

      state.OnUpdate();

      AssertStateChange<SingleJumpStart>();
    }


    [Test]
    public void RollEnd_Can_Land() {
      SetupTest();

      movementSettings.IdleThreshold = 1;
      state.OnStateAdded();

      physics.Velocity = new Vector2(0.5f, 0);

      state.OnFixedUpdate();

      AssertStateChange<Land>();
    }


    [Test]
    public void RollEnd_Can_Crouch() {
      SetupTest();

      player.TryingToMove().Returns(false);
      player.HoldingDown().Returns(true);

      state.OnRollEndFinished();

      AssertStateChange<Crouching>();
    }

    [Test]
    public void RollEnd_Can_Idle(){
      SetupTest();

      player.TryingToMove().Returns(false);
      player.HoldingDown().Returns(false);

      state.OnRollEndFinished();

      AssertStateChange<Idle>();
    }

    [Test]
    public void RollEnd_Can_Crawl() {
      SetupTest();

      player.TryingToMove().Returns(true);
      player.CanMove().Returns(true);
      player.HoldingDown().Returns(true);

      state.OnRollEndFinished();

      AssertStateChange<Crawling>();
    }

    [Test]
    public void RollEnd_Can_Run() {
      SetupTest();

      player.TryingToMove().Returns(true);
      player.CanMove().Returns(true);
      player.HoldingDown().Returns(false);

      state.OnRollEndFinished();

      AssertStateChange<Running>();
    }

  }
}
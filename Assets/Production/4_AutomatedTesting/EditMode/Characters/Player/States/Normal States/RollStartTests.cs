using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using HumanBuilders;



namespace HumanBuilders.Tests {
  public class RollStartTests : PlayerStateTest<RollStart> {

    [Test]
    public void RollStart_Can_Jump() {
      SetupTest();

      player.HoldingJump().Returns(true);
      player.PressedJump().Returns(true);

      state.OnUpdate();

      AssertStateChange<SingleJumpStart>();
    }

    [Test]
    public void RollStart_Can_SlowToUncrouch() {
      SetupTest();

      settings.IdleThreshold = 1;
      state.OnStateAdded();

      physics.Velocity = new Vector2(0, 0);
      state.OnFixedUpdate();

      AssertStateChange<CrouchEnd>();
    }


    [Test]
    public void RollStart_Can_TransitionToCrouching() {
      SetupTest();
      
      player.GetHorizontalInput().Returns(0);
      player.HoldingDown().Returns(true);

      state.OnRollStartFinished();

      AssertStateChange<Crouching>();
    }

    [Test]
    public void RollStart_Can_StopToUncrouch() {
      SetupTest();

      player.GetHorizontalInput().Returns(0);
      player.HoldingDown().Returns(false);

      state.OnRollStartFinished();

      AssertStateChange<CrouchEnd>();
    }

    [Test]
    public void RollStart_Can_Crawl() {
      SetupTest();
    
      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);
      player.HoldingDown().Returns(true);

      state.OnRollStartFinished();

      // assert
      AssertStateChange<Crawling>();
    }

    [Test]
    public void RollStart_CantMove_Can_EndRollEarly() {
      SetupTest();
    
      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(false);
      player.HoldingDown().Returns(true);

      state.OnRollStartFinished();

      AssertStateChange<RollEnd>();
    }

    [Test]
    public void RollStart_NoHoldingDown_Can_EndRollEarly() {
      SetupTest();

      player.GetHorizontalInput().Returns(1);
      player.CanMove().Returns(true);
      player.HoldingDown().Returns(false);

      state.OnRollStartFinished();

      AssertStateChange<RollEnd>();
    }
  }
}
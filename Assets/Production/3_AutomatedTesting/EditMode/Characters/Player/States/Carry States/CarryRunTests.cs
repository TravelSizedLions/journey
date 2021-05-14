using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;


using HumanBuilders;

namespace HumanBuilders.Tests {

  public class CarryRunTests : PlayerStateTest<CarryRun> {
    [Test]
    public void Can_Start_Jump_While_Carrying() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.PressedJump().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<CarryJumpStart>();
    }

    [Test]
    public void Can_Start_Crouch_While_Carrying() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.PressedJump().Returns(false);
      player.PressedDown().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<CarryCrouchStart>();
    }

    [Test]
    public void Can_Idle_While_Carrying() {
      SetupTest();
      
      settings.IdleThreshold = 0.05f;
      state.OnStateAdded();

      player.GetHorizontalInput().Returns(0);
      player.Physics.Vx = 1f;

      player.IsTouchingGround().Returns(true);
      player.IsFalling().Returns(false);
      player.PressedAction().Returns(false);

      for (int i = 0; i < 20; i++) {
        state.OnFixedUpdate();
      }
      
      AssertStateChange<CarryIdle>();
    }

    [Test]
    public void Can_Fall_While_Carrying() {
      SetupTest();
      
      player.Facing.Returns(Facing.Left);
      player.IsTouchingGround().Returns(false);
      player.IsFalling().Returns(true);

      state.OnFixedUpdate();
      
      AssertStateChange<CarryJumpFall>();
    }

    [Test]
    public void Can_Throw_Item() {
      SetupTest();
      
      player.Facing.Returns(Facing.Left);
      player.IsTouchingGround().Returns(true);
      player.IsFalling().Returns(false);
      player.ReleasedAction().Returns(true);

      state.OnUpdate();
      state.OnFixedUpdate();

      player.HoldingAction().Returns(true);
      player.ReleasedAction().Returns(false);
      player.PressedAction().Returns(true);

      state.OnUpdate();
      state.OnFixedUpdate();
      
      AssertStateChange<ThrowItem>();
    }
  }
}
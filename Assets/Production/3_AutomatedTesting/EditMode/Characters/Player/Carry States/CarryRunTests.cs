using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;

namespace Tests.Characters.Player {

  public class CarryRunTests : PlayerStateTest<CarryRun> {
    [Test]
    public void Can_Start_Jump_While_Carrying() {
      SetupTest();
      
      player.PressedJump().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<CarryJumpStart>();
    }

    [Test]
    public void Can_Start_Crouch_While_Carrying() {
      SetupTest();
      
      player.PressedJump().Returns(false);
      player.PressedDown().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<CarryCrouchStart>();
    }

    [Test]
    public void Can_Idle_While_Carrying() {
      SetupTest();
      
      movementSettings.IdleThreshold = 0.05f;
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
      
      player.IsTouchingGround().Returns(false);
      player.IsFalling().Returns(true);

      state.OnFixedUpdate();
      
      AssertStateChange<CarryJumpFall>();
    }

    [Test]
    public void Can_Throw_Item() {
      SetupTest();
      
      player.IsTouchingGround().Returns(true);
      player.IsFalling().Returns(false);

      player.PressedAction().Returns(true);

      state.OnFixedUpdate();
      
      AssertStateChange<ThrowItem>();
    }
  }
}
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;


namespace Tests.Characters.Player {
  public class CarryJumpFallTests : PlayerStateTest<CarryJumpFall> {

    [Test]
    public void Can_Jump_With_Coyote_Time_While_Carrying() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.PressedJump().Returns(true);
      player.InCoyoteTime().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<CarryJumpStart>();
    }

    [Test]
    public void Can_Throw_Item_In_Midair() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.PressedJump().Returns(false);
      player.ReleasedAction().Returns(true);

      state.OnUpdate();

      player.PressedAction().Returns(true);
      player.HoldingAction().Returns(true);

      state.OnUpdate();

      AssertStateChange<MidAirThrowItem>();
    }

    [Test]
    public void Can_Run_While_Carrying() {
      SetupTest();
      
      player.IsTouchingGround().Returns(true);
      player.TryingToMove().Returns(true);

      state.OnFixedUpdate();
      
      AssertStateChange<CarryRun>();
    }

    [Test]
    public void Can_Start_Crouching_While_Carrying() {
      SetupTest();
      
      player.IsTouchingGround().Returns(true);
      player.TryingToMove().Returns(false);
      player.HoldingDown().Returns(true);

      state.OnFixedUpdate();
      
      AssertStateChange<CarryCrouchStart>();
    }

    [Test]
    public void Can_Land_While_Carrying() {
      SetupTest();
      
      player.IsTouchingGround().Returns(true);
      player.TryingToMove().Returns(false);
      player.HoldingDown().Returns(false);

      state.OnFixedUpdate();
      
      AssertStateChange<CarryLand>();
    }

    [Test]
    public void CarryJumpFall_No_Op() {
      SetupTest();
      
      player.PressedJump().Returns(false);
      player.PressedAction().Returns(false);
      player.IsTouchingGround().Returns(false);


      AssertNoStateChange<CarryJumpStart>();
      AssertNoStateChange<MidAirThrowItem>();
      AssertNoStateChange<CarryRun>();
      AssertNoStateChange<CarryCrouchStart>();
      AssertNoStateChange<CarryLand>();
    }

  }
}

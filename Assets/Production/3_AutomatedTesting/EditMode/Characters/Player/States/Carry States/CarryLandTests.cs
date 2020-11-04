using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;
using Storm.Characters.Player;


namespace Tests.Characters.Player {
  public class CarryLandTests : PlayerStateTest<CarryLand> {

    [Test]
    public void Can_Start_Jump_While_Carrying() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.PressedJump().Returns(true);
      state.OnUpdate();
      
      AssertStateChange<CarryJumpStart>();
    }

    [Test]
    public void Can_Drop_Item() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.PressedJump().Returns(false);
      player.ReleasedAction().Returns(true);

      state.OnUpdate();

      player.HoldingAction().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<DropItem>();
    }

    [Test]
    public void Can_Run_While_Carrying() {
      SetupTest();
      
      player.TryingToMove().Returns(true);

      state.OnFixedUpdate();
      
      AssertStateChange<CarryRun>();
    }

    
    [Test]
    public void Can_Idle_While_Carrying() {
      SetupTest();
      
      state.OnCarryLandFinished();
      
      AssertStateChange<CarryIdle>();
    }

    [Test]
    public void CarryLand_No_Op() {
      SetupTest();

      player.CarriedItem = BuildCarriable();
      player.PressedJump().Returns(false);
      player.HoldingAction().Returns(false);
      player.TryingToMove().Returns(false);

      state.OnUpdate();
      state.OnFixedUpdate();

      AssertNoStateChange<CarryJumpStart>();
      AssertNoStateChange<DropItem>();
      AssertNoStateChange<CarryRun>();
      AssertNoStateChange<CarryIdle>();      
    }

  }
}
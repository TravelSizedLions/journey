using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;

namespace Tests.Characters.Player {
  public class CarryIdleTests : PlayerStateTest<CarryIdle> {

    [Test]
    public void Can_Run_While_Carrying() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.TryingToMove().Returns(true);

      state.OnUpdate();

      AssertStateChange<CarryRun>();
    }

    [Test]
    public void Can_Drop_Item() {
      SetupTest();
            
      player.CarriedItem = BuildCarriable();
      player.TryingToMove().Returns(false);

      player.ReleasedAction().Returns(true);
      state.OnUpdate();
      player.PressedAction().Returns(true);
      player.HoldingAction().Returns(true);

      state.OnUpdate();

      AssertStateChange<DropItem>();
    }

    [Test]
    public void Can_Start_Jump_While_Carrying() {
      SetupTest();
            
      player.CarriedItem = BuildCarriable();
      player.TryingToMove().Returns(false);
      player.PressedJump().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<CarryJumpStart>();
    }

    [Test]
    public void Can_Start_Crouch_While_Carrying() {
      SetupTest();
            
      player.CarriedItem = BuildCarriable();
      player.TryingToMove().Returns(false);
      player.PressedAction().Returns(false);
      player.ReleasedAction().Returns(false);
      player.PressedJump().Returns(false);
      player.PressedDown().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<CarryCrouchStart>();
    }
  }
}
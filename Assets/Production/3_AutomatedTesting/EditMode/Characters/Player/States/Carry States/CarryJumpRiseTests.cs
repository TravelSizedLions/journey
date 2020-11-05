using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;
using Storm.Characters.Player;

namespace Tests.Characters.Player {
  public class CarryJumpRiseTests : PlayerStateTest<CarryJumpRise> {

    [Test]
    public void Can_Throw_Item_In_Midair() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.ReleasedAction().Returns(true);

      state.OnUpdate();

      player.PressedAction().Returns(true);
      player.HoldingAction().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<MidAirThrowItem>();
    }


    [Test]
    public void Can_Fall_While_Carrying() {
      SetupTest();
      
      player.IsFalling().Returns(true);

      state.OnFixedUpdate();
      
      AssertStateChange<CarryJumpFall>();
    }

    [Test]
    public void CarryJumpRise_No_Op() {
      SetupTest();
      
      player.PressedAction().Returns(true);
      player.IsFalling().Returns(false);

      state.OnFixedUpdate();
      state.OnUpdate();

      AssertNoStateChange<MidAirThrowItem>();
      AssertNoStateChange<CarryJumpFall>();      
    }

  }
}

